using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("SubSuppliers")]
    public class SubSupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SubSupplierId { get; set; }

        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public string SubSupplierName { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
