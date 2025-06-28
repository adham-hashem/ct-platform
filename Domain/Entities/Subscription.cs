using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public SubscriptionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public List<int> SubscribedMonths { get; set; } = new List<int>(); // 1-12
        [Required]
        public EducationalLevel Grade { get; set; } // FirstYear, SecondYear, ThirdYear
    }
}
