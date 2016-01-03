using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class TeachersController : ApiController
    {
        private readonly ScheduleContext _db = new ScheduleContext();

        public IEnumerable<Teacher> Get()
        {
            return _db.Teachers.ToArray();
        }

        public dynamic Get(int id)
        {
            var teacher = _db.Teachers.FirstOrDefault(t => t.Id == id);
            var lessons = _db.Lessons.Where(l => l.TeacherId == id).ToArray();
            dynamic lessonsData = from l in lessons
                           select new  
                           {
                               l.Id, l.Number, l.Name,
                               l.DayOfWeek, l.NumberOfWeek,
                               l.Cabinet, l.GroupId, l.Group,
                               TeacherId = l.Teacher.Id
                           };

            dynamic data = new { Teacher = teacher, Lessons = lessonsData };
            return data; 
        } 
    }
}