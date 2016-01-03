using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class LessonsController : ApiController
    {
        private readonly ScheduleContext _db = new ScheduleContext();

        public IEnumerable<Lesson> Get()
        {
            return _db.Lessons.ToArray();
        }
    }
}
