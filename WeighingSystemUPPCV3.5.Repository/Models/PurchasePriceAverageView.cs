using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("PurchasePriceAverageViews")]
    [Keyless]
    public class PurchasePriceAverageView
    {

        public DateTime DT{ get; set; }

        public long RawMaterialId { get; set; }

        public string RawMaterialDesc { get; set; }

        public long CategoryId { get; set; }

        public int RecordCount { get; set; }

        public decimal NetWtTotal { get; set; }

        public decimal AdjustedWtTotal { get; set; }

        public decimal WeightedPriceAvg { get; set; }

        public decimal StraightAvg { get; set; }

        public int DTYear { get; set; }

        public int DTMonth { get; set; }

    }
}
