using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Lesson
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [StringLength(500)]
        public string VideoUrl { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        [Range(1, 12)]
        public int? MonthAssigned { get; set; } // 1-12
        [StringLength(1000)]
        public string? AdditionalResources { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
