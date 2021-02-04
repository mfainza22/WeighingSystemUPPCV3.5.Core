using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WeighingSystemUPPCV3_5_Repository.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Bales")]
    public class BalesInvBale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BaleId { get; set; }

        public string BaleCode { get; set; }

    }
}
