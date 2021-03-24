using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class PurchaseOrder
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PurchaseOrderId { get; set; }

        public DateTime DTEffectivity { get; set; }

        [Range(1, 9999999999, ErrorMessage = "Supplier is required")]
        public long SupplierId { get; set; }

        [Range(1, 9999999999, ErrorMessage = "Material is required")]
        public long RawMaterialId { get; set; }

        [Required(ErrorMessage = " P.O. number is required ", AllowEmptyStrings = false)]
        [MaxLength(20, ErrorMessage = "P.O. number length must not exceed to 20 characters")]
        
        public string PONum { get; set; }

        public decimal Price { get; set; }

        [DefaultValue(0)]
        [Range(1, 9999999999, ErrorMessage = "Total Balance(Kg) is required")]
        public decimal BalanceTotalKg { get; set; }

        [Required(ErrorMessage = "P.O. Type is required.")]
        [MaxLength(20, ErrorMessage = "P.O. Type length must not exceed to 20 characters")]
        public string POType { get; set; }

        [MaxLength(200, ErrorMessage = "Remarks length must not exceed to 200 characters")]
        public string Remarks { get; set; }

        public DateTime DTCreated { get; set; }

        [MaxLength(100, ErrorMessage = "CreatedById length must not exceed to 100 characters")]
        public string CreatedById { get; set; }

        public Nullable<DateTime> DTModified { get; set; }

        [MaxLength(100, ErrorMessage = "ModifiedById length must not exceed to 100 characters")]
        public string ModifiedById { get; set; }

        public bool? Locked { get; set; }

        public bool? IsActive { get; set; }

        public string RawMaterialDesc { get; set; }


        public virtual PurchaseOrderView PurchaseOrderView { get; set; }

        //[ForeignKey("PurchaseOrderId")]
        //public virtual ICollection<Inyard> Inyards { get; set; }

        //[ForeignKey("PurchaseOrderId")]
        //public virtual ICollection<PurchaseTransaction> PurchaseTransactions{ get; set; }

    }
}
