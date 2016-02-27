using System.Data.Entity;

namespace Schedule.Models.Student_Schedule_Models
{
    public class StudentScheduleContext : DbContext
    {
        public StudentScheduleContext() 
            : base("AzureDatabase")
        { }

        public DbSet<StudentGroup> Groups { get; set; }
        public DbSet<StudentTeacher> Teachers { get; set; }
        public DbSet<StudentLesson> Lessons { get; set; }
    }
}