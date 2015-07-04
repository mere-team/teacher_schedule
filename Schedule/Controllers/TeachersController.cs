using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherSchedule.Models;
using Schedule.Helpers;

namespace Schedule.Controllers
{
    public class TeachersController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        public JsonResult Get(int id)
        {
            var lessons = _db.Lessons
                .Where(l => l.TeacherId == id).ToArray();

            var dataLessons = from l in lessons
                           select new  
                           {
                               Id = l.Id,
                               Number = l.Number,
                               Name = l.Name,
                               DayOfWeek = l.DayOfWeek,
                               NumberOfWeek = l.NumberOfWeek,
                               Cabinet = l.Cabinet,
                               GroupId = l.GroupId,
                               Group = l.Group 
                           };

            var dataTeacher = _db.Teachers
                .FirstOrDefault(t => t.Id == id);

            var Obj = new { teacher = dataTeacher, lessons = dataLessons };

            JsonResult json = new JsonResult { Data = Obj, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

            return json; 
        } 
    }
}