using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{

    [Table("InTheMachine")]
    public class InTheMachine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SeqNo { get; set; }

        public string CATID { get; set; }

        public decimal ITHWeight { get; set; }

        public decimal MC { get; set; }

        public DateTime DT { get; set; }

        public decimal WeekDay { get; set; }

        public decimal WeekNo { get; set; }

        public DateTime FirstDay { get; set; }

        public DateTime LastDay { get; set; }

    }
}
