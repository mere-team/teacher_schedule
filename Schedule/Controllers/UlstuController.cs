using System.Linq;
using System.Web.Mvc;
using TeacherSchedule.Models;
using Newtonsoft.Json;

namespace Schedule.Controllers
{
    public class UlstuController : Controller
    {
        private readonly ScheduleContext _db = new ScheduleContext();

        public string Cathedra(int id) => ToJson(_db.Cathedries.Find(id));
        public string Cathedries() => ToJson(_db.Cathedries.ToArray());
        public string FacultyCathedries(int id) => ToJson(_db.Cathedries.Where(c => c.FacultyId == id).ToArray());

        public string Faculty(int id) => ToJson(_db.Faculties.Find(id));
        public string Faculties() => ToJson(_db.Faculties.ToArray());

        public string Group(int id) => ToJson(_db.Groups.Find(id));
        public string Groups() => ToJson(_db.Groups.ToArray());

        public string Teacher(int id) => ToJson(_db.Teachers.Find(id));
        public string Teachers() => ToJson(_db.Teachers.ToArray());
        public string TeacherLessons(int id)
        {
            var lessons = _db.Lessons.Where(l => l.TeacherId == id).ToArray();
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
                                      TeacherId = l.Teacher.Id
                                  }).ToArray();
            return ToJson(lessonsData);
        }

        public string Lesson(int id) => ToJson(_db.Lessons.Find(id));
        public string Lessons() => ToJson(_db.Lessons.ToArray());
        
        private static string ToJson<T>(T model) => JsonConvert.SerializeObject(model);
    }
}