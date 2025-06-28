using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(1000)]
        public string? Qualifications { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public EducationalLevel? Grade { get; set; } // FirstYear, SecondYear, ThirdYear
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public bool IsOnline { get; set; }
        public double VideoCompletionRate { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public List<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
