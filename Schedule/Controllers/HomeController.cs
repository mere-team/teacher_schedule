using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebSockets;
using Schedule.Helpers;
using Schedule.Models;
using Schedule.Models.Student_Schedule_Models;
using Schedule.Parsers;

namespace Schedule.Controllers
{
    public class HomeController : Controller
    {
        private readonly ScheduleContext _db = new ScheduleContext();

        public ActionResult Index()
        {
            ViewBag.Title = "УлГТУ. Расписание преподавателей.";

            try
            {
                ViewBag.Faculties = _db.Faculties.ToList();
            }
            catch (Exception ex)
            {
                Logger.E(ex);
            }

            return View();
        }

        public FileResult TestStudentsSchedule()
        {
            var parser = new StudentScheduleParser();

            var groups = parser.GetGroups().ToList();
            var lessons = parser.GetSchedule().ToList();

            return GetResult(groups, lessons);
        }

        private FileResult GetResult(List<StudentGroup> groups, List<StudentLesson> lessons)
        {
            string serverPath = System.Web.HttpContext.Current.Server.MapPath("") + "+";
            var temp = serverPath.Split('\\').Last();
            serverPath = serverPath.Replace(temp, "");
            string fileName = "result.txt";
            string path = serverPath + fileName;
            var resultFile = System.IO.File.Open(path, FileMode.Create, FileAccess.ReadWrite);
            var sw = new StreamWriter(resultFile);

            sw.WriteLine(Logger.LogMessages);
            sw.WriteLine("========================================= ВЫВОД ДАННЫХ ============================================\r\n");
            foreach (var group in groups)
            {
                var info = new StringBuilder();
                info.AppendFormat("Group: {0} \r\n\r\n", group.Name);

                foreach (var l in lessons.Where(l => l.Group.Name == group.Name))
                {
                    info.AppendFormat("N: {0,-2}   N: {1,-36}  D: {2,-2}  WN: {3,-2}  C: {4,-26}  G: {5,-12}  T: {6,-46}\r\n",
                        l.Number,
                        l.Name,
                        l.DayOfWeek,
                        l.NumberOfWeek,
                        l.Cabinet,
                        l.Group.Name,
                        l.Teacher?.Name);
                }
                info.Append("-------------------------------------------------------------------------------------------------\r\n\n");
                sw.WriteLine(info.ToString());
            }
            sw.WriteLine("=========================================== КОНЕЦ ВЫВОДA ==============================================");
            sw.Close();

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}
