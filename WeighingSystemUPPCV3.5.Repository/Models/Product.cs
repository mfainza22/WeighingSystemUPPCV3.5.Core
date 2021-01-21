using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductId { get; set; }

        public string ProductIdOld { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code is required.")]
        public string ProductCode { get; set; }

        [MaxLength(50, ErrorMessage = "Description must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required.")]
        public string ProductDesc { get; set; }

        public decimal Price { get; set; }

        public Nullable<short> SeqNum { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<long> CategoryId { get; set; }

        [NotMapped]
        public string CategoryDesc { get; set; }

        public virtual Category Category { get; set; }
    }
}
