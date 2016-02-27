using System.Data.Entity;

namespace Schedule.Models
{
    internal class ScheduleContext : DbContext
    {
        public ScheduleContext()
            : base("AzureDatabase")
        {
        }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Cathedra> Cathedries { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
    }
}