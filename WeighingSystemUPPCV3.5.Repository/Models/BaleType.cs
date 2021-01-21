using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BaleTypes")]
    public class BaleType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BaleTypeId { get; set; }

        [MaxLength(8, ErrorMessage = "Id must not exceed to 20 characters.")]
        public string BaleTypeIdOld { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        public string BaleTypeCode { get; set; }

        [MaxLength(50, ErrorMessage = "Description must not exceed to 20 characters.")]
        public string BaleTypeDesc { get; set; }

        public Nullable<bool> IsActive { get; set; }
    }
}
