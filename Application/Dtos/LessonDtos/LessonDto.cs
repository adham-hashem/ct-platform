using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.LessonDtos
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string VideoUrl { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        public int MonthAssigned { get; set; }
        public Guid CourseId { get; set; }
    }
}
