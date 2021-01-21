using WeighingSystemUPPCV3_5_Repository.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class PurchaseOrderView : IPurchaseOrder
    {
        [Key]
        public long PurchaseOrderId { get; set; }
        public DateTime DTEffectivity { get; set; }
        public long SupplierId { get; set; }
        public long RawMaterialId { get; set; }
        public string PONum { get; set; }
        public decimal BalanceTotalKg { get; set; }
        public string POType { get; set; }
        public string Remarks { get; set; }
        public DateTime DTCreated { get; set; }
        public string CreatedById { get; set; }
        public DateTime? DTModified { get; set; }
        public string ModifiedById { get; set; }
        public bool? Locked { get; set; }
        public bool? IsActive { get; set; }
        public string RawMaterialDesc { get; set; }

        public decimal? BalanceRemainingKg { get; set; }

        public decimal? BalanceDeliveredKg { get; set; }
    }
}
