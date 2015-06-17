using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherSchedule.Models
{
    class ScheduleContext : DbContext
    {
        //public ScheduleContext() : base()
        //{

        //}

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Cathedra> Cathedries { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
