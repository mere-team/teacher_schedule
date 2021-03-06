﻿using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Schedule.Models;
using Schedule.Models.Student_Schedule_Models;

namespace Schedule.Controllers
{
    public class UlstuTeacherController : Controller
    {
        private readonly ScheduleContext _teacherDb = new ScheduleContext();
        private readonly StudentScheduleContext _studentDb = new StudentScheduleContext();

        public string Cathedra(int id) => ToJson(_teacherDb.Cathedries.Find(id));
        public string Cathedries() => ToJson(_teacherDb.Cathedries.ToArray());
        public string FacultyCathedries(int id) => ToJson(_teacherDb.Cathedries.Where(c => c.FacultyId == id).ToArray());

        public string Faculty(int id) => ToJson(_teacherDb.Faculties.Find(id));
        public string Faculties() => ToJson(_teacherDb.Faculties.ToArray());

        public string Group(int id) => ToJson(_teacherDb.Groups.Find(id));
        public string Groups() => ToJson(_teacherDb.Groups.ToArray());

        public string Teacher(int id) => ToJson(_teacherDb.Teachers.Find(id));
        public string Teachers() => ToJson(_teacherDb.Teachers.ToArray());
        public string TeacherLessons(int id)
        {
            var lessons = _teacherDb.Lessons.Where(l => l.TeacherId == id).ToArray();
            dynamic lessonsData = (from l in lessons
                                  select new
                                  {
                                      l.Id,
                                      l.Number,
                                      l.Name,
                                      l.DayOfWeek,
                                      l.NumberOfWeek,
                                      l.Cabinet,
                                      l.GroupId,
                                      l.Group,
                                      TeacherId = l.Teacher.Id
                                  }).ToArray();
            return ToJson(lessonsData);
        }

        public string Lesson(int id) => ToJson(_teacherDb.Lessons.Find(id));
        public string Lessons() => ToJson(_teacherDb.Lessons.ToArray());

        
        private static string ToJson<T>(T model) => JsonConvert.SerializeObject(model);
    }
}