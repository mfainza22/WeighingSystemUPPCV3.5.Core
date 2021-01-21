using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("CalibrationTypes")]
    public class CalibrationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalibrationTypeId { get; set; }

        [MaxLength(30, ErrorMessage = "Description must not exceed to 30 characters.")]
        public string CalibrationTypeDesc { get; set; }

        public Nullable<bool> IsActive { get; set; }

    }
}
