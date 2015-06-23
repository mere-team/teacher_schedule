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
            var faculties = _db.Faculties.ToArray().Distinct(new FacultyComparer()).ToArray(); 
            
            return faculties;
        }

        public IEnumerable<Teacher> Get(int id)
        {
            var teachers = _db.Teachers
                .Where(t => t.FacultyId == id).ToArray()
                .Distinct(new TeacherComparer()).ToArray();

            return teachers;
        }

        
    }

    public class FacultyComparer : IEqualityComparer<Faculty>
    {
        public bool Equals(Faculty x, Faculty y)
        {
            if (x.Name == y.Name)
                return true;
            return false;
        }

        public int GetHashCode(Faculty obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class CathedraComparer : IEqualityComparer<Cathedra>
    {
        public bool Equals(Cathedra x, Cathedra y)
        {
            if (x.Name == y.Name)
                return true;
            return false;
        }

        public int GetHashCode(Cathedra obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class TeacherComparer : IEqualityComparer<Teacher>
    {
        public bool Equals(Teacher x, Teacher y)
        {
            if (x.Name == y.Name)
                return true;
            return false;
        }

        public int GetHashCode(Teacher obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class LessonComparer : IEqualityComparer<Lesson>
    {
        public bool Equals(Lesson x, Lesson y)
        {
            if (x.GetHashCode() == y.GetHashCode())
                return true;
            return false;
        }

        public int GetHashCode(Lesson o)
        {
            var lesson = new StringBuilder();
            lesson.Append(o.Cabinet).Append(o.DayOfWeek).Append(o.GroupId).Append(o.Name).Append(o.Number)
                .Append(o.NumberOfWeek).Append(o.TeacherId);
            return o.ToString().GetHashCode();
        }
    }

    public class GroupComparer : IEqualityComparer<Group>
    {
        public bool Equals(Group x, Group y)
        {
            if (x.Name == y.Name)
                return true;
            return false;
        }

        public int GetHashCode(Group obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}