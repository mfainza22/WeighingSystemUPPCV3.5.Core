using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CustomerId { get; set; }

        public string CustomerIdOld { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code is required.")]
        [Remote("ValidateCode", AdditionalFields = "CustomerId", ErrorMessage = "Code must be unique.")]
        public string CustomerCode { get; set; }

        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public string CustomerName { get; set; }

        [MaxLength(100, ErrorMessage = "Location must not exceed to 100 characters.")]
        public string Location { get; set; }

        [MaxLength(50, ErrorMessage = "Contact Person must not exceed to 50 characters.")]
        public string ContactPerson { get; set; }

        [MaxLength(20, ErrorMessage = "Contact Number must not exceed to 20 characters.")]
        public string ContactNum { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
