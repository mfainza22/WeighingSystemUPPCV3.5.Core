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

        public Nullable<long> VehicleTypeId { get; set; }

        public Nullable<long> CustomerId { get; set; }

        public Nullable<long> SupplierId { get; set; }

        public Nullable<long> HaulerId { get; set; }

        public bool IsActive { get; set; }


        [ForeignKey(nameof(Models.Vehicle.VehicleTypeId))]
        public virtual VehicleType VehicleType { get; set; }

        [ForeignKey(nameof(Models.Vehicle.CustomerId))]
        public virtual Customer Customer { get; set; }

        [ForeignKey(nameof(Models.Supplier.SupplierId))]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey(nameof(Models.Hauler.HaulerId))]
        public Hauler Hauler { get; set; }

        [NotMapped]
        public string VehicleTypeCode { get; set; }
    }
}
