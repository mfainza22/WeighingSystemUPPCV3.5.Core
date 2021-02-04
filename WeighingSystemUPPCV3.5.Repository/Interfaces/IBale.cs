using System;
using System.Collections.Generic;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    public interface IBale
    {
        long BaleId { get; set; }

        DateTime DT { get; set; }

        string BaleCode { get; set; }
        int BaleNum { get; set; }

        long CategoryId { get; set; }

        long ProductId { get; set; }

        int BaleWt { get; set; }

        int BaleWt10 { get; set; }

        public string Remarks { get; set; }

        public string FIFORemarks { get; set; }

        public bool InStock { get; set; }

        public bool IsReject { get; set; }

        public DateTime? DTDelivered { get; set; }

        public string ProductDesc { get; set; }

        public string CategoryDesc { get; set; }

        public DateTime DTCreated { get; set; }

    }
}
