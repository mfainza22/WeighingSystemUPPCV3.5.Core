using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("ReferenceNumbers")]
    public class ReferenceNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReferenceNumberId { get; set; }

        [MaxLength(8, ErrorMessage = "Inyard Ref. Number must not exceed to 8 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Inyard Ref. Number is required.")]
        public string InyardNum { get; set; }

        [MaxLength(8, ErrorMessage = "Purchases Receipt Number must not exceed to 8 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Purchases Receipt Number is required.")]
        public string PurchaseReceiptNum { get; set; }

        [MaxLength(8, ErrorMessage = "Sales Receipt Number must not exceed to 8 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sales Receipt Number is required.")]
        public string SaleReceiptNum { get; set; }
    }
}
