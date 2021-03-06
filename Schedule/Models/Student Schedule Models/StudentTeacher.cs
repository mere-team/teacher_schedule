﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Schedule.Models.Student_Schedule_Models
{
    public class StudentTeacher
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentLesson> Lessons { get; set; }

        public override string ToString()
        {
            return "Teacher: Id=" + Id + ", Name=" + Name;
        }
    }
}