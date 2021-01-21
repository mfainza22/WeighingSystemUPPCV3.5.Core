using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("PurchaseSaleMCComparisonViews")]
    public class PurchaseSalesMCComparisonView
    {
        [Key]
        public long? Id { get; set; }

        public DateTime DateTimeOut { get; set; }

        public long? CategoryId { get; set; }

        public string CategoryDesc { get; set; }

        public decimal? PurchaseNetWt { get; set; }

        public decimal? PurchaseCorrected10 { get; set; }

        public decimal? PurchaseCorrected12 { get; set; }

        public decimal? PurchaseMCAvg { get; set; }

        public decimal? SaleNetWt { get; set; }

        public decimal? SaleCorrected10 { get; set; }

        public decimal? SaleCorrected12 { get; set; }

        public decimal? SaleMCAvg { get; set; }

        public decimal? PaymentWt { get; set; }

    }
}
