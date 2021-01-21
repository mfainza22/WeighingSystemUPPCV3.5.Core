using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("PrintLogs")]
    public class PrintLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PrintLogId { get; set; }

        public DateTime DTPrinted { get; set; }

        public long TransactionId { get; set; }

        public string TransactionTypeCode { get; set; }

        public string ReceiptNum { get; set; }

        public string PrintReasons { get; set; }

        public string UserAccountId { get; set; }

        public string UserAccountName { get; set; }
    }
}
