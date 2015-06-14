using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherScheduleParser.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public int FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }

        [Required]
        public int CathedraId { get; set; }
        public virtual Cathedra Cathedra { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
