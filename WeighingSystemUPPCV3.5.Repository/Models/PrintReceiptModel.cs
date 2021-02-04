using System;
using System.Collections.Generic;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class PrintReceiptModel
    {

        public long TransactionId { get; set; }

        public string TransactionTypeCode { get; set; }

        public string ReceiptNum { get; set; }

        public string ReprintRemarks { get; set; }

        public bool IsReprinted { get; set; }

        public string UserAccoountId { get; set; }

        public string UserFullName { get; set; }
    }
}
