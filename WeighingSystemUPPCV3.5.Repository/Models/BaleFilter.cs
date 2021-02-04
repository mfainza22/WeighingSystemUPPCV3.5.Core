using System;
using SysUtility.Enums;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class BaleFilter
    {
        public long BaleId { get; set; }
        public string BaleCode { get; set; }

        public BaleStatus BaleStatus { get; set; }

        public long SaleId { get; set; }

        public DateTime? DTCreatedFrom { get; set; }
        public DateTime? DTCreatedTo { get; set; }

        public DateTime? DTDeliveredFrom { get; set; }

        public DateTime? DTDeliveredTo { get; set; }

        public long CategoryId { get; set; }

        public long ProductId { get; set; }

        public string SearchText { get; set; }

        public int InventoryAge { get; set; }

        public bool AsListView { get; set; }

    }
}
