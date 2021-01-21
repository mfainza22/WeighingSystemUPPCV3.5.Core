using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SupplierId { get; set; }

        public string SupplierIdOld { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code is required.")]
        public string SupplierCode { get; set; }

        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public string SupplierName { get; set; }

        [MaxLength(100, ErrorMessage = "Location must not exceed to 100 characters.")]
        public string Location { get; set; }
        [MaxLength(50, ErrorMessage = "Contact Person must not exceed to 50 characters.")]
        public string ContactPerson { get; set; }

        [MaxLength(20, ErrorMessage = "Contact Number must not exceed to 20 characters.")]
        public string ContactNum { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
