using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("ReportDays")]
    public class ReportDay
    {
        [Key]
        public DateTime? DT { get; set; }

        public int? WeekNum { get; set; }
        public int? WeekDay { get; set; }
        public DateTime? FirstDay{ get; set; }
        public DateTime? LastDay { get; set; }
    }
}
