using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{

    public class TVFInventory
    {
        [Key]
        public DateTime? DT { get; set; }

        public long? CategoryId { get; set; }

        public int? BeginningBaleWt { get; set; }

        public int? BeginningBaleCount { get; set; }

        public int PurchaseBaleWtTotal { get; set; }

        public int PurchaseBaleWtTotalCumulative { get; set; }

        public int PurchaseBaleCountTotal { get; set; }

        public int PurchaseBaleCountTotalCumulative { get; set; }

        public int SaleBaleWtTotal { get; set; }

        public int SaleBaleWtTotalCumulative { get; set; }

        public int SaleBaleCountTotal { get; set; }

        public int SaleBaleCountTotalCumulative { get; set; }

        public int? EndingBaleWt { get; set; }

        public int? EndingBaleCount { get; set; }

    }
}
