using WeighingSystemUPPCV3_5_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SysUtility.Enums;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("PurchaseTransactions")]
    public class PurchaseTransaction : IInyard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PurchaseId { get; set; }

        [MaxLength(8, ErrorMessage = "ReceiptNum length must not exceed to 8 characters")]
        public string ReceiptNum { get; set; }

        public DateTime DateTimeIn { get; set; }

        public DateTime? DateTimeOut { get; set; }

        [Required(ErrorMessage = " VehicleNum is required ", AllowEmptyStrings = false)]
        [MaxLength(25, ErrorMessage = "VehicleNum length must not exceed to 25 characters")]
        public string VehicleNum { get; set; }

        [Required(ErrorMessage = " Trip is required ", AllowEmptyStrings = false)]
        [MaxLength(15, ErrorMessage = "Trip length must not exceed to 15 characters")]
        public string Trip { get; set; }

        [Range(1, 999999999, ErrorMessage = "Supplier is required.")]
        public long SupplierId { get; set; }

        [Range(1, 999999999, ErrorMessage = "Material is required.")]
        public long RawMaterialId { get; set; }

        [Range(1, 999999999, ErrorMessage = "Bale type is required.")]

        public long BaleTypeId { get; set; }

        public long CategoryId { get; set; }

        [DefaultValue(0)]
        public int BaleCount { get; set; }

        [MaxLength(15, ErrorMessage = "P.O. Number length must not exceed to 15 characters")]
        public string PONum { get; set; }

        [MaxLength(15, ErrorMessage = "D.R Number length must not exceed to 15 characters")]
        public string DRNum { get; set; }

        [DefaultValue(0)]
        public decimal Price { get; set; }

        [DefaultValue(0)]
        public decimal FactoryWt { get; set; }
        [DefaultValue(0)]
        public decimal GrossWt { get; set; }
        [DefaultValue(0)]
        public decimal TareWt { get; set; }
        [DefaultValue(0)]
        public decimal NetWt { get; set; }
        [DefaultValue(0)]
        public decimal MC { get; set; }
        [DefaultValue(0)]
        public decimal PM { get; set; }

        [DefaultValue(0)]
        public decimal OT { get; set; }
        [DefaultValue(0)]
        public int MCStatus { get; set; }
        [DefaultValue(0)]
        public decimal Corrected10 { get; set; }
        [DefaultValue(0)]
        public decimal Corrected12 { get; set; }
        [DefaultValue(0)]
        public decimal Corrected14 { get; set; }
        [DefaultValue(0)]
        public decimal Corrected15 { get; set; }

        [MaxLength(50, ErrorMessage = "DriverName length must not exceed to 50 characters")]
        public string DriverName { get; set; }

        [MaxLength(100, ErrorMessage = "WeigherInId length must not exceed to 100 characters")]
        public string WeigherInId { get; set; }

        [MaxLength(100, ErrorMessage = "WeigherOutId length must not exceed to 100 characters")]
        public string WeigherOutId { get; set; }

        [MaxLength(50, ErrorMessage = "SubSupplierName length must not exceed to 50 characters")]
        public string SubSupplierName { get; set; }

        [MaxLength(200, ErrorMessage = "Remarks length must not exceed to 200 characters")]
        public string Remarks { get; set; }

        public long? MoistureReaderId { get; set; }

        [Required(ErrorMessage = " SourceId is required ", AllowEmptyStrings = false)]
        public long SourceId { get; set; }

        [Required(ErrorMessage = " SourceCategoryId is required ", AllowEmptyStrings = false)]
        public long SourceCategoryId { get; set; }

        [Required(ErrorMessage = " Time zone in is required ", AllowEmptyStrings = false)]
        [MaxLength(20, ErrorMessage = "TimeZoneIn length must not exceed to 20 characters")]
        public string TimeZoneIn { get; set; }

        [Required(ErrorMessage = " Time zone Out is required ", AllowEmptyStrings = false)]
        [MaxLength(20, ErrorMessage = "TimeZoneOut length must not exceed to 20 characters")]
        public string TimeZoneOut { get; set; }
        [DefaultValue(false)]
        public bool IsOfflineIn { get; set; }
        [DefaultValue(false)]
        public bool? IsOfflineOut { get; set; }


        public int WeekDay { get; set; }


        public int WeekNum { get; set; }

        public DateTime? FirstDay { get; set; }

        public DateTime? LastDay { get; set; }

        public long SignatoryId { get; set; }

        public long MoistureSettingsId { get; set; }
        [DefaultValue(0)]
        public int PrintCount { get; set; }

        public Nullable<long> VehicleTypeId { get; set; }

        [MaxLength(25, ErrorMessage = "Vehicle Type must not exceed to 25 characters")]
        public string VehicleTypeCode { get; set; }

        [Required(ErrorMessage = " SourceName is required ", AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "SourceName length must not exceed to 50 characters")]
        public string SourceName { get; set; }

        [MaxLength(50, ErrorMessage = "SourceCategoryDesc length must not exceed to 50 characters")]
        public string SourceCategoryDesc { get; set; }

        [MaxLength(50, ErrorMessage = "BaleTypeDesc length must not exceed to 50 characters")]
        public string BaleTypeDesc { get; set; }

        [MaxLength(50, ErrorMessage = "Supplier Name length must not exceed to 50 characters")]
        public string SupplierName { get; set; }

        [MaxLength(50, ErrorMessage = "Raw Material Description length must not exceed to 50 characters")]
        public string RawMaterialDesc { get; set; }

        [MaxLength(50, ErrorMessage = "Category Descriptin length must not exceed to 50 characters")]
        public string CategoryDesc { get; set; }

        [MaxLength(50, ErrorMessage = "Weigher-in Name length must not exceed to 50 characters")]
        public string WeigherInName { get; set; }

        [MaxLength(50, ErrorMessage = "Weigher-out Name length must not exceed to 50 characters")]
        public string WeigherOutName { get; set; }

        public string MoistureReaderProcess { get; set; }

        [MaxLength(50, ErrorMessage = "Moisture Reader Description length must not exceed to 50 characters")]
        public string MoistureReaderDesc { get; set; }


        [MaxLength(3, ErrorMessage = "Baling Station Num length must not exceed to 50 characters")]
        public string BalingStationNum { get; set; }
        [MaxLength(50, ErrorMessage = "Baling Station Code length must not exceed to 50 characters")]
        public string BalingStationCode{ get; set; }

        [MaxLength(50, ErrorMessage = "Baling Station Name length must not exceed to 50 characters")]
        public string BalingStationName { get; set; }
        
        public Nullable<DateTime> MCDate { get; set; }

        [NotMapped]
        public long InyardId { get; set; }

        [NotMapped]
        public bool MoistureReaderLogsModified { get; set; }

        [ForeignKey("TransactionId")]
        public virtual ICollection<MoistureReaderLog> MoistureReaderLogs { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateOut { get; set; }

        public virtual RawMaterial RawMaterial { get; set; }
    }
}