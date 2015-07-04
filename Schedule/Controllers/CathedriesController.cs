using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class CathedriesController : ApiController
    {
        private ScheduleContext db = new ScheduleContext();

        public IEnumerable<Cathedra> Get()
        {
            var cathedries = db.Cathedries.ToArray();

            return cathedries;
        }

        public IEnumerable<Teacher> Get(int id)
        {
            var teachers = db.Teachers
                .Where(t => t.CathedraId == id).ToArray();

            return teachers;
        }
    }
}
