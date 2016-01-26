using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TeacherSchedule.Models;

namespace Schedule.Models
{
    public class Cathedra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }

        [JsonIgnore]
        public virtual ICollection<Teacher> Teachers { get; set; }

        public Cathedra()
        {
            Teachers = new List<Teacher>();
        }
    }
}
