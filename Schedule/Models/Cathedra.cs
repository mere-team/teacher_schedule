using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherSchedule.Models
{
    public class Cathedra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Teacher> Teachers { get; set; }

        public Cathedra()
        {
            Teachers = new List<Teacher>();
        }
    }
}
