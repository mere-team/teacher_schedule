using System;
using Schedule.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeacherSchedule.Models;
using Schedule.Controllers;

namespace TeacherSchedule.Controllers
{
    public class HomeController : Controller
    {
        private ScheduleContext db = new ScheduleContext();
        public ActionResult Index()
        {
            ViewBag.Title = "УлГТУ. Расписание преподавателей.";
            //db.Database.Delete();
            //db.Database.CreateIfNotExists();
            //db.Database.Initialize(true);
            //db.Faculties.ToList();
            //var t = db.Cathedries.ToList();
            //db.Groups.ToList();
            //db.Teachers.ToList();
            //db.Lessons.ToList();
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

            ViewBag.Faculties = db.Faculties.ToList();

            return View();
        }

        
        public JsonResult Faculties()
        {
            var faculties = db.Faculties.ToArray();
            var list = from f in faculties
                       select new
                       {
                           Id = f.Id,
                           Name = f.Name
                       };

            var json = Json(list);
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }
    }
}
