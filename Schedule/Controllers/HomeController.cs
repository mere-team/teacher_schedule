using System.Linq;
using System.Web.Mvc;
using Schedule.Models;

namespace Schedule.Controllers
{
    public class HomeController : Controller
    {
        private readonly ScheduleContext _db = new ScheduleContext();
        public ActionResult Index()
        {
            ViewBag.Title = "УлГТУ. Расписание преподавателей.";
//                        _db.Database.Delete();
//                        _db.Database.CreateIfNotExists();
//                        _db.Database.Initialize(true);
//            Database.SetInitializer(new DropCreateDatabaseAlways<ScheduleContext>());
//            Database.SetInitializer(new DropCreateDatabaseAlways<ApplicationDbContext>());
//            Database.SetInitializer(new DropCreateDatabaseAlways<StudentScheduleContext>());
// Drop migration history table if not works

            ViewBag.Title = "УлГТУ. Расписание преподавателей. data udated";
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
