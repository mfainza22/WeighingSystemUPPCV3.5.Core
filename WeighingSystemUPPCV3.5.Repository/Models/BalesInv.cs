using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BalesInv")]
    public class BalesInv
    {
       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RefId { get; set; }

        public string BaleId { get; set; }

      
        public string CatId { get; set; }

      
        public string ProductId { get; set; }

        public DateTime? FirstDay { get; set; }

        public DateTime? LastDay { get; set; }

        public decimal? WeekDay { get; set; }

        public decimal? WeekNo { get; set; }

        public DateTime? Rdate { get; set; }

        public decimal? BaleWt { get; set; }

        public decimal? BaleWt10 { get; set; }

        public string Type { get; set; }

        public string StationId { get; set; }

        public decimal? MC1 { get; set; }

        public decimal? MC2 { get; set; }

        public decimal? MC3 { get; set; }

        public decimal? MC4 { get; set; }

        public decimal? MC5 { get; set; }

        public decimal? MC6 { get; set; }

        [MaxLength(80, ErrorMessage = "Remarks length must not exceed to 80 characters")]
        public string Remarks { get; set; }

        public bool? Reject { get; set; }

        public bool? Delivered { get; set; }

        public string SalesId { get; set; }

        public string DYR { get; set; }

        public string DMNT { get; set; }

        public DateTime? DTCREATED { get; set; }

        public DateTime? DTDELIVERED { get; set; }

    }

}
