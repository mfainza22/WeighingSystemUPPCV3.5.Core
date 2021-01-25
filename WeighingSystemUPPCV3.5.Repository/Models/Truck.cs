using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Truck")]
    public class Truck
    {
       [Key]
        public string PlateNo { get; set; }

        [MaxLength(8, ErrorMessage = "CustomerId length must not exceed to 8 characters")]
        public string CustomerId { get; set; }

        [MaxLength(8, ErrorMessage = "SupplierId length must not exceed to 8 characters")]
        public string SupplierId { get; set; }

        [MaxLength(8, ErrorMessage = "HaulerID length must not exceed to 8 characters")]
        public string HaulerID { get; set; }

        [MaxLength(25, ErrorMessage = "TruckCode length must not exceed to 25 characters")]
        public string TruckCode { get; set; }

    }
}
