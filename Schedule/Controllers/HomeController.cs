using System;
﻿using Schedule.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeacherSchedule.Models;

namespace TeacherSchedule.Controllers
{
    public class HomeController : Controller
    {
        private ScheduleContext db = new ScheduleContext();
        public ActionResult Index()
        {
            ViewBag.Title = "Home";

            db.Faculties.ToList();
            var t = db.Cathedries.ToList();
            db.Groups.ToList();
            db.Teachers.ToList();
            db.Lessons.ToList();

            using (var downloader = new ExcelDocumentDownloader())
            {
                var docs = downloader.DownloadDocuments();
                foreach (var doc in docs)
                {
                    var parser = new ScheduleParser(doc);
                    parser.GetTeachersSchedules();
                    parser.Dispose();
                }
            }

            return View();
        }
    }
}
