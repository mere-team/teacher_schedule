using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class FacultyController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        public ICollection<Faculty> Get()
        {
            return _db.Faculties.ToList();
        }
    }
}