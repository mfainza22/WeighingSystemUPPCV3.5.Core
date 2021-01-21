using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Vehicles")]
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VehicleId { get; set; }


        [MaxLength(20, ErrorMessage = "Vehicle number must not exceed to 20 characters.")]
        [Required(ErrorMessage = "Vehicle Numbers is required")]
        public string VehicleNum { get; set; }

        public long VehicleTypeId { get; set; }

        public string CustomerIdOld { get; set; }

        public Nullable<long> CustomerId { get; set; }

        public string SupplierIdOld { get; set; }

        public Nullable<long> SupplierId { get; set; }

        public string HaulerIdOld { get; set; }

        public Nullable<long> HaulerId { get; set; }

        [NotMapped]
        public string VehicleTypeDesc { get; set; }
        public Nullable<bool> IsActive { get; set; }

        [ForeignKey("VehicleTypeId")]
        public virtual VehicleType VehicleType { get; set; }
    }
}
