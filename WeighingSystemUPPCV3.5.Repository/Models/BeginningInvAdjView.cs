using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("BeginningInvAdjViews")]
    public class BeginningInvAdjView
    {
        [Key]
        public long BeginningInvAdjId { get; set; }

        public long CategoryId { get; set; }
        public DateTime DT { get; set; }

        public int? WtActual { get; set; }

        public int? Wt { get; set; }

        public int? Wt10 { get; set; }

        public int? BaleCount { get; set; }

        public int? DYear { get; set; }

        public int? DMonth { get; set; }

        public int WeekNum { get; set; }

        public int WeekDay { get; set; }

        public DateTime? FirstDay { get; set; }

        public DateTime? LastDay { get; set; }

    }
}
