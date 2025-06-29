using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.CourseDtos
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        [Required]
        public string EducationalLevel { get; set; } = string.Empty;
        [Required]
        public string ShortDescription { get; set; } = string.Empty;
        public string DetailedDescription { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string WhatStudentsWillLearn { get; set; } = string.Empty;
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    }
}
