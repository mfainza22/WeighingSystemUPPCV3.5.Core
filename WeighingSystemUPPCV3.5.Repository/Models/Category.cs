using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryId { get; set; }

        public string CategoryIdOld { get; set; }

        [DefaultValue("")]
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        [Remote("ValidateCode", "Categories", HttpMethod = "POST", ErrorMessage = "Code already exists")]
        public string CategoryCode { get; set; }

        [DefaultValue("")]
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        [Remote("ValidateName", "Categories", HttpMethod = "POST", ErrorMessage = "Description already exists")]
        public string CategoryDesc { get; set; }

        [DefaultValue(0)]
        public Nullable<short> SeqNum { get; set; }

        [DefaultValue(true)]
        public Nullable<bool> IsActive { get; set; }


    }
}
