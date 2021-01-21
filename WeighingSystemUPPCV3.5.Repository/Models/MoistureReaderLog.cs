using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("MoistureReaderLogs")]
    public class MoistureReaderLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MoistureReaderLogId { get; set; }



        public long TransactionId { get; set; }
        public int LogNum { get; set; }

        public DateTime? DTLog { get; set; }

        public decimal MC { get; set; }
    }
}
