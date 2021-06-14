using System;
using System.Collections.Generic;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class PurchaseOrderFilter
    {
        public string PONum { get; set; }

        public string POType { get; set; }

        public bool? Available { get; set; }

        public DateTime? DTEffectivity { get; set; }

        public bool IncludeTotals { get; set; }

        public long SupplierId{ get; set; }

        public long RawMaterialId { get; set; }

        public bool? IsActive { get; set; }
    }
}
