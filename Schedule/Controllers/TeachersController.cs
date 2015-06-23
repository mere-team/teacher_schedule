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
        public IEnumerable<Teacher> Get()
        {
            return _db.Teachers.ToArray().Distinct(new TeacherComparer()).ToArray();
        }

        public IEnumerable<Lesson> Get(int id)
        {
            var lessons = _db.Lessons
                .Where(l => l.TeacherId == id).ToArray()
                .Distinct(new LessonComparer()).ToArray();

            return lessons;
        } 
    }
}