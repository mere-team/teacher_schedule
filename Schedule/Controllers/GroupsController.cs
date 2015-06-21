using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TeacherSchedule.Models;

namespace Schedule.Controllers
{
    public class GroupsController : ApiController
    {
        private ScheduleContext _db = new ScheduleContext();

        // GET: Groups
        public ICollection<Group> Get()
        {
            return _db.Groups.ToList(); 
        }

        public List<Lesson> Get(int id)
        {
            var lessons = _db.Lessons.Where(l => l.GroupId == id).ToList();

            return lessons;
        }
    }
}