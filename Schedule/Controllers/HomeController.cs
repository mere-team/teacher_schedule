using System;
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
            ViewBag.Title = "Home Page";

            db.Faculties.ToList();
            var t = db.Cathedries.ToList();
            db.Groups.ToList();
            db.Teachers.ToList();
            db.Lessons.ToList();

            return View();
        }
    }
}
