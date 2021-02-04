using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BalingStationsStatusView")]
    public class BalingStationStatusView
    {
        [Key]
        public string BalingStationNum { get; set; }

        public decimal ActualHoldings { get; set; }

        public decimal HighestPrice { get; set; }
        public decimal InventoryValue { get; set; }

        public decimal Stocks70 { get; set; }

        public decimal Stocks90 { get; set; }

    }
}
