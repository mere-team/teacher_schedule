using System.Linq;
using System.Web.Http;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class TeachersController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        public dynamic Get(int id)
        {
            var teacher = _db.Teachers.FirstOrDefault(t => t.Id == id);
            var lessons = _db.Lessons.Where(l => l.TeacherId == id).ToArray();
            dynamic lessonsData = from l in lessons
                           select new  
                           {
                               Id = l.Id,
                               Number = l.Number,
                               Name = l.Name,
                               DayOfWeek = l.DayOfWeek,
                               NumberOfWeek = l.NumberOfWeek,
                               Cabinet = l.Cabinet,
                               GroupId = l.GroupId,
                               Group = l.Group,
                               TeacherId = l.Teacher.Id
                           };

            dynamic data = new { Teacher = teacher, Lessons = lessonsData };
            return data; 
        } 
    }
}