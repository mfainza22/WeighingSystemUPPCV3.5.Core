using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WeighingSystemUPPCV3_5_Repository.Models
{

    public class CalibrationLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalibrationLogId { get; set; }

        [Required(ErrorMessage = " CalibrationId is required ", AllowEmptyStrings = false)]
        public long CalibrationId { get; set; }

        public DateTime DTScheduledCalibration { get; set; }

        [Required(ErrorMessage = " DTActualCalibration is required ", AllowEmptyStrings = false)]
        public DateTime DTActualCalibration { get; set; }

        [MaxLength(100, ErrorMessage = "CalibratedBy length must not exceed to 100 characters")]
        public string CalibratedBy { get; set; }

        [Required(ErrorMessage = " Date Confirmed is required ", AllowEmptyStrings = false)]
        public DateTime DTConfirmed { get; set; }

        public string ConfirmedBy { get; set; }


    }

}
