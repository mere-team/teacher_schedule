using System.Data.Entity;

namespace Schedule.Models.Student_Schedule_Models
{
    public class StudentScheduleContext : DbContext
    {
        public StudentScheduleContext() 
            : base("AzureConnection")
        { }

        public DbSet<StudentGroup> Groups { get; set; }
        public DbSet<StudentTeacher> Teachers { get; set; }
        public DbSet<StudentLesson> Lessons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentLesson>()
                .Property(p => p.Cabinet)
                .HasMaxLength(30);
        }
    }
}