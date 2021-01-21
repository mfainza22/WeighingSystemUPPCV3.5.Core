using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("AuditLogEvents")]
    public class AuditLogEvent
    {
        [Key]
        public long AuditLogEventId { get; set; }

        public string Description { get; set; }

    }
}
