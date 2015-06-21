using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherSchedule.Models;
using Newtonsoft.Json;
using System.Web.Http.Results;

namespace Schedule.Controllers
{
    public class FacultiesController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        public List<Faculty> Get()
        {
            var faculties = _db.Faculties.ToList();
            
            return faculties;
        }

        public List<Teacher> Get(int id)
        {
            var teachers = _db.Teachers.Where(t => t.FacultyId == id).ToList();

            return teachers;
        }
    }
}