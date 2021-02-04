using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("SaleBales")]
    public class SaleBale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SaleBaleId { get; set; }
            
        public Nullable<long> SaleId { get; set; }

        public Nullable<long> BaleId { get; set; }

        [NotMapped]
        public int Process { get; set; }

        [ForeignKey("BaleId")]
        public virtual Bale Bale { get; set; }
    }
}
