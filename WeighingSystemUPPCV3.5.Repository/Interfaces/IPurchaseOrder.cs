using System;
using System.Collections.Generic;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    public interface IPurchaseOrder
    {
        long PurchaseOrderId { get; set; }

        DateTime DTEffectivity { get; set; }

        long SupplierId { get; set; }

        long RawMaterialId { get; set; }

        string PONum { get; set; }

        decimal BalanceTotalKg { get; set; }

        string POType { get; set; }

        string Remarks { get; set; }

        DateTime DTCreated { get; set; }

        string CreatedById { get; set; }

        Nullable<DateTime> DTModified { get; set; }

        public string ModifiedById { get; set; }

        bool? Locked { get; set; }

        bool? IsActive { get; set; }

        string RawMaterialDesc { get; set; }
    }
}
