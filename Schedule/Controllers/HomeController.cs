using System.Linq;
using System.Web.Mvc;
using Schedule.Models;
using TeacherSchedule.Models;
using Schedule.Parsers;

namespace Schedule.Controllers
{
    public class HomeController : Controller
    {
        private readonly ScheduleContext _db = new ScheduleContext();
        public ActionResult Index()
        {
            ViewBag.Title = "УлГТУ. Расписание преподавателей.";
            //_db.Database.Delete();
            //_db.Database.CreateIfNotExists();
            //_db.Database.Initialize(true);
            //_db.Faculties.ToList();
            //var t = _db.Cathedries.ToList();
            //_db.Groups.ToList();
            //_db.Teachers.ToList();
            //_db.Lessons.ToList();
            //using (var downloader = new ExcelDocumentDownloader())
            //{
            //    var docs = downloader.DownloadDocuments();
            //    foreach (var doc in docs)
            //    {
            //        var parser = new ScheduleParser(doc);
            //        parser.GetTeachersSchedules();
            //        parser.SaveDataInDatabase();
            //        parser.Dispose();
            //    }
            //}
            StudentScheduleParser parser = new StudentScheduleParser();
            ViewBag.Faculties = _db.Faculties.ToList();

            return View();
        }

        
        public JsonResult Faculties()
        {
            var faculties = _db.Faculties.ToArray();
            var list = from f in faculties
                       select new
                       {
                           f.Id, f.Name
                       };

            var json = Json(list);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }
    }
}
