using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{

    [Table("Tbl_PO")]
    public class Tbl_PO
    {
        [Key]
        public long SeqNo { get; set; }

        public string SupplierId { get; set; }

        public string MaterialId { get; set; }

        public string PONo { get; set; }

        public decimal? TotalBal { get; set; }

        public decimal? RemBal { get; set; }

        public decimal? AutoRemBal { get; set; }

        public bool? Selected { get; set; }

        public DateTime? PODate { get; set; }

        public DateTime? PODateMod { get; set; }

        public string memo { get; set; }

        public string POType { get; set; }

        public string CreatedBy { get; set; }

    }

}
