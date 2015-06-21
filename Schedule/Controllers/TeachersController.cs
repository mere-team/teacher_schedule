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
        public List<Teacher> Get()
        {
            return _db.Teachers.ToList();
        }

        public List<Lesson> Get(int id)
        {
            var lessons = _db.Lessons.Where(l => l.TeacherId == id).ToList();

            return lessons;
        } 
    }
}