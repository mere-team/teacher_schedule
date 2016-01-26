using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Schedule.Models.Student_Schedule_Models
{
    public class StudentTeacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentLesson> Lessons { get; set; }
    }
}