﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherScheduleParser.Models
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        
        public virtual ICollection<Teacher> Teachers { get; set; }

        public Faculty()
        {
            Teachers = new List<Teacher>();
        }
    }
}