using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("PurchaseGrossWtRestrictions")]
    public class PurchaseGrossWtRestriction
    {

        public PurchaseGrossWtRestriction(string vehicleNum = null, decimal weight = 0)
        {
            VehicleNum = vehicleNum;
            Weight = weight;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PurchaseGrossWtRestrictionId { get; set; }

        public string VehicleNum { get; set; }

        public decimal Weight { get; set; }

        public DateTime DTRestriction { get; set; }

        [NotMapped]
        public DateTime DateTimeIn { get; set; }

    }
}
