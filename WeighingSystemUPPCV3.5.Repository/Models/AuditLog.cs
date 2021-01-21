using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuditLogId { get; set; }

        public DateTime DTLog { get; set; }

        public string UserAccountId { get; set; }

        [Range(1,999999999, ErrorMessage = "Event is required.")]
        public long AuditLogEventId { get; set; }

        [MaxLength(50, ErrorMessage = "Event Description must not exceed to 50 characters.")]
        public string AuditLogEventDesc { get; set; }

        [MaxLength(300, ErrorMessage = "Notes must not exceed to 300 characters.")]
        public string Notes { get; set; }

    }
}
