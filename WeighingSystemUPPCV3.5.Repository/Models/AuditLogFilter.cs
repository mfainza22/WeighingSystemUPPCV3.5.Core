using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class AuditLogFilter
    {
        public Nullable<DateTime> DTLogFrom { get; set; }

        public Nullable<DateTime> DTLogTo { get; set; }

        public string UserAccountId { get; set; }

        public int AuditLogEventId { get; set; }

    }
}
