using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Schedule.Models.Student_Schedule_Models;

namespace Schedule.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentScheduleContext db = new StudentScheduleContext();

        public string Group(int id) => ToJson(db.Groups.Find(id));
        public string Groups() => ToJson(db.Groups.ToArray());

        public string Teacher(int id) => ToJson(db.Teachers.Find(id));
        public string Teachers() => ToJson(db.Teachers.ToArray());

        public string Lesson(int id) => ToJson(db.Lessons.Find(id));
        public string Lessons() => ToJson(db.Lessons.ToArray());

        public string GroupLessons(int id) => ToJson(db.Lessons.Where(l => l.GroupId == id).ToArray());


        private static string ToJson<T>(T model) => JsonConvert.SerializeObject(model);
    }
}