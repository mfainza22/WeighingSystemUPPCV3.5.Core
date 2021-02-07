using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WeighingSystemUPPCV3_5_Repository.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BalesInventoryViews")]
    public class BaleInventoryView 
    {
        [Key]
        public long BaleId { get; set; }

        public bool InStock { get; set; }
        public int InventoryAge { get; set; }

        public Nullable<DateTime> DTDelivered { get; set; }


    }
}
