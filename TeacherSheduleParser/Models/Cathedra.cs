using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherSheduleParser.Models
{
    public class Cathedra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Teacher> Teachers { get; set; }

        public Cathedra()
        {
            Teachers = new List<Teacher>();
        }
    }
}
