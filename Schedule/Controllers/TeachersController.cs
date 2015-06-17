using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class TeachersController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        // GET: Teacher
        public ICollection<Teacher> Get(params int[] list)
        {
            if (list == null)
                return _db.Teachers.ToList();
            else
                return _db.Faculties.Where(c => c.Id == list[0]).FirstOrDefault().Teachers;
        }
    }
}