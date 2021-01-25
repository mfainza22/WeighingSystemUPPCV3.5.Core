using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WeighingSystemUPPCV3_5_Repository.Models
{

    public class Calibration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CalibrationId { get; set; }

        public long? ItemNum { get; set; }

        [Range(1, 9999999, ErrorMessage = "Calibration type is required ")]
        public long CalibrationTypeId { get; set; }

        [Required(ErrorMessage = " Description is required ", AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "Description must not exceed to 50 characters.")]
        public string Description { get; set; }

        public DateTime? DTLastCalibration { get; set; }

        [Required(ErrorMessage = " Frequency is required ", AllowEmptyStrings = false)]
        public int Frequency { get; set; }

        [Required(ErrorMessage = " DTNextCalibration is required ", AllowEmptyStrings = false)]
        public DateTime DTNextCalibration { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? DTReminder { get; set; }

        [MaxLength(100, ErrorMessage = "Owner length must not exceed to 100 characters")]
        public string Owner { get; set; }

        public DateTime? DTLastActualCalibration { get; set; }

        public DateTime? DTLastConfirmed { get; set; }


        [MaxLength(100, ErrorMessage = "CalibratedBy length must not exceed to 100 characters")]
        public string CalibratedBy { get; set; }

        public virtual CalibrationType CalibrationType { get; set; }

        [NotMapped]
        public CalibrationLog LastLog { get; set; }

    }

}
