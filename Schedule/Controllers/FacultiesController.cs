using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TeacherSchedule.Models;
using System.Text;

namespace Schedule.Controllers
{
    public class FacultiesController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        public IEnumerable<Faculty> Get()
        {
            var faculties = _db.Faculties.ToArray(); 
            
            return faculties;
        }

        public IEnumerable<Cathedra> Get(int id)
        {
            var cathedries = _db.Cathedries
                .Where(c => c.Id == id).ToArray();

            return cathedries;
        }
    }
}