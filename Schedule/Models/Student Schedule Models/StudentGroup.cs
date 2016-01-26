using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Schedule.Models.Student_Schedule_Models
{
    public class StudentGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentLesson> Lessons { get; set; }
    }
}