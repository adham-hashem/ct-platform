using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        [Required]
        public PaymentMethodType PaymentMethod { get; set; }
        [StringLength(100)]
        public string NameOnInvoice { get; set; } = string.Empty;
        [StringLength(200)]
        public string BillingAddress { get; set; } = string.Empty;
        [StringLength(50)]
        public string TaxNumber { get; set; } = string.Empty;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
