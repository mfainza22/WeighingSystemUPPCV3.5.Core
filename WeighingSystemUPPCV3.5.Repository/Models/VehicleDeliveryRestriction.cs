using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("VehicleDeliveryRestrictions")]
    public class VehicleDeliveryRestriction
    {
        public VehicleDeliveryRestriction(string vehicleNum = null, long commodityId = 0)
        {
            VehicleNum = vehicleNum;
            CommodityId = commodityId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VehicleDeliveryRestrictionId { get; set; }

        public string VehicleNum { get; set; }

        public long CommodityId { get; set; }

        public DateTime DTRestriction { get; set; }

        [NotMapped]
        public DateTime DateTimeIn { get; set; }

    }
}
