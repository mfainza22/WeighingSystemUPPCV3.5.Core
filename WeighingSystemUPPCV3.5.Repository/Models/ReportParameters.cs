using System;
using SysUtility.Enums;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class ReportParameters
    {

        public ReportType ReportType { get; set; }

        public DateTime DTFrom { get; set; }

        public DateTime DTTo { get; set; }

        public long ClientId { get; set; }

        public long CommodityId { get; set; }

        public long CategoryId { get; set; }

        public string PreparedBy { get; set; }

        public BalingStation BalingStation { get; set; }

        public ReportPaperSize PaperSize { get; set; }

        public string TransactionTypeCode { get; set; }

    }
}
