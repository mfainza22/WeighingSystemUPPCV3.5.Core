using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{

    [Table("ReturnedVehicles")]
    public class ReturnedVehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ReturnedVehicleId { get; set; }

        public long SaleId { get; set; }

        public DateTime DTArrival { get; set; }

        public decimal PlantNetWt { get; set; }

        public decimal MC { get; set; }


        public decimal Corrected10 { get; set; }

        public decimal Corrected12 { get; set; }

        public decimal Corrected14 { get; set; }

        public decimal PM { get; set; }

        public decimal OT { get; set; }

        public decimal PMAdjustedWt { get; set; }

        public decimal OTAdjustedWt { get; set; }

        public decimal DiffNet { get; set; }

        public decimal DiffCorrected10 { get; set; }

        public decimal DiffCorrected12 { get; set; }

        public decimal BaleCount { get; set; }

        public int DiffDay { get; set; }

        public decimal DiffTime { get; set; }


        [Required(ErrorMessage = "Remarks is required ", AllowEmptyStrings = false)]
        [MaxLength(200, ErrorMessage = "Remarks length must not exceed to 200 characters")]
        public string Remarks { get; set; }

        [MaxLength(200, ErrorMessage = "Time Variance Remarks length must not exceed to 200 characters")]
        public string TimeVarianceRemarks { get; set; }

        [Required(ErrorMessage = "Origin is required ", AllowEmptyStrings = false)]
        [MaxLength(100, ErrorMessage = "Origin length must not exceed to 100 characters")]
        public string VehicleOrigin { get; set; }

        public DateTime? DTGuardIn { get; set; }

        public DateTime? DTGuardOut { get; set; }

        public DateTime? DTOutToPlant { get; set; }


        [MaxLength(100, ErrorMessage = "User Account Id length must not exceed to 100 characters")]
        public string UserAccountId { get; set; }

        [MaxLength(60, ErrorMessage = "User Account Full Name length must not exceed to 60 characters")]
        public string UserAccountFullName { get; set; }

    }

}
