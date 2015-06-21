using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class CathedriesController : ApiController
    {
        private ScheduleContext db = new ScheduleContext();

        public List<Cathedra> Get()
        {
            var cathedries = db.Cathedries.ToList();

            return cathedries;
        }

        public List<Teacher> Get(int id)
        {
            var teachers = db.Teachers.Where(t => t.CathedraId == id).ToList();

            return teachers;
        }
    }
}
