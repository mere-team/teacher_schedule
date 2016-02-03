using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Schedule.Models.Student_Schedule_Models;

namespace Schedule.Controllers
{
    public class UlstuStudentController : Controller
    {
        private readonly StudentScheduleContext _db = new StudentScheduleContext();

        public string Group(int id) => ToJson(_db.Groups.Find(id));
        public string Groups() => ToJson(_db.Groups.ToArray());

        public string Teacher(int id) => ToJson(_db.Teachers.Find(id));
        public string Teachers() => ToJson(_db.Teachers.ToArray());

        public string Lesson(int id) => ToJson(_db.Lessons.Find(id));
        public string Lessons() => ToJson(_db.Lessons.ToArray());

        public string GroupLessons(int id)
        {
            var lessons = _db.Lessons.Where(l => l.GroupId == id).ToArray();
            dynamic lessonsData = (from l in lessons
                                   select new
                                   {
                                       l.Id,
                                       l.Number,
                                       l.Name,
                                       l.DayOfWeek,
                                       l.NumberOfWeek,
                                       l.Cabinet,
                                       l.GroupId,
                                       l.Group,
                                       TeacherId = l.Teacher.Id,
                                       l.Teacher
                                   }).ToArray();
            return ToJson(lessonsData);
        }


        private static string ToJson<T>(T model) => JsonConvert.SerializeObject(model);
    }
}