using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WeighingSystemUPPCV3_5_Repository.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BalesViews")]
    public class BaleView 
    {
        [Key]
        public long SaleBaleId { get; set;}

        public long BaleId { get; set; }

        public long SaleId { get; set; }

        public long DTDelivered { get; set; }

    }
}
