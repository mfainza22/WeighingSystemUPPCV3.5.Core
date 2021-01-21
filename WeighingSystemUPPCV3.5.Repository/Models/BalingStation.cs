using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BalingStations")]
    public class BalingStation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BalingStationId { get; set; }

        [MaxLength(3, ErrorMessage = "Baling Station Number must not exceed to 20 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Baling Station Number is required.")]
        public string BalingStationNum { get; set; }

        [MaxLength(20, ErrorMessage = "Code must not exceed to 20 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code is required.")]
        public string BalingStationCode { get; set; }

        [MaxLength(50, ErrorMessage = "Name must not exceed to 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
        public string BalingStationName { get; set; }

        [MaxLength(100, ErrorMessage = "Location must not exceed to 100 characters.")]
        public string Location { get; set; }

        [MaxLength(50, ErrorMessage = "Area Head must not exceed to 50 characters.")]
        public string AreaHead { get; set; }

        [MaxLength(50, ErrorMessage = "Department Manager  must not exceed to 50 characters.")]
        public string DepartmentManager { get; set; }

        public decimal WarehouseHoldings { get; set; }

        public decimal InsuranceCoverage { get; set; }

        public DateTime DateEstablished { get; set; }
        public DateTime DateCreated { get; set; }
        public Nullable<DateTime> DateModified { get; set; }
        public bool Selected { get; set; }
        public Nullable<bool> IsActive { get; set; }


        [NotMapped]
        public int CalibrationDay { get; set; }

        public bool ReceivingLocked { get; set; }
    }
}
