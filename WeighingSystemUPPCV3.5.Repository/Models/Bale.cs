using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WeighingSystemUPPCV3_5_Repository.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Bales")]
    public class Bale 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BaleId { get; set; }

        public DateTime DT { get; set; }

        [MaxLength(30, ErrorMessage = "BaleCode length must not exceed to 30 characters")]
        
        public string BaleCode { get; set; }

        [Range(0, 9999, ErrorMessage = "Bale Number must not exceed to 4 characters")]
        public int BaleNum { get; set; }

        public long CategoryId { get; set; }

        public long ProductId { get; set; }

        [Range(1, 99999, ErrorMessage = "Invalid bale weight.")]
        public int BaleWt { get; set; }

        public int BaleWt10 { get; set; }

        [MaxLength(100, ErrorMessage = "Remarks length must not exceed to 100 characters")]
        public string Remarks { get; set; }

        [DefaultValue("First In, First Out")]
        public string FIFORemarks { get; set; }

        public bool IsReject { get; set; }

        public string ProductDesc { get; set; }

        public string CategoryDesc { get; set; }

        public DateTime DTCreated { get; set; }

        public virtual SaleBale SaleBale { get; set; }

        public virtual BaleInventoryView BaleInventoryView { get; set; }

    }
}
