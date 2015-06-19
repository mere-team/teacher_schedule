using System.ComponentModel.DataAnnotations;

namespace TeacherSchedule.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 8, ErrorMessage = "Number of lesson must be in range 1 to 8")]
        public int Number { get; set; }

        [Required]
        [MaxLength(40, ErrorMessage = "Too long name of lesson")]
        public string Name { get; set; }

        [Required]
        [Range(1, 7, ErrorMessage = "Day of week must be in range 1 to 7")]
        public int DayOfWeek { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Number of week must be in range 1 to 2")]
        public int NumberOfWeek { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Too long name of cabinet")]
        public string Cabinet { get; set; }

        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }

        [Required]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
