using System;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class TransactionFilter
    {

        public long TransactionId { get; set; }

        public string ReceiptNum { get; set; }

        public DateTime? DTFrom { get; set; }

        public DateTime? DTTo { get; set; }


        /// <summary>
        /// For Sale Transaction Only
        /// </summary>
        public Nullable<bool> Returned { get; set; }
    }
}
