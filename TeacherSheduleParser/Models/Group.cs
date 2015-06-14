using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherSheduleParser.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Name { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }
    }
}
