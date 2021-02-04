using WeighingSystemUPPCV3_5_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SysUtility.Enums;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Inyards")]
    public class Inyard : IInyard, ICloneable
    {
        public Inyard()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InyardId { get; set; }


        [Required(ErrorMessage = "Inyard Number is required ", AllowEmptyStrings = false)]
        [MaxLength(8, ErrorMessage = "Inyard Number length must not exceed to 8 characters")]
        public string InyardNum { get; set; }


        [MaxLength(2, ErrorMessage = "Transaction Type Code must not exceed to 10 characters")]
        public string TransactionTypeCode { get; set; }


        [Required(ErrorMessage = "DateTimeIn is required ", AllowEmptyStrings = false)]
        public DateTime DateTimeIn { get; set; }

        [NotMapped]
        public DateTime? DateTimeOut { get; set; }

        [Required(ErrorMessage = "Vehicle Number is required ", AllowEmptyStrings = false)]
        [MaxLength(25, ErrorMessage = "Vehicle Number length must not exceed to 25 characters")]
        public string VehicleNum { get; set; }


        [Required(ErrorMessage = "Type of Trip is required ", AllowEmptyStrings = false)]
        [MaxLength(15, ErrorMessage = "Type of Trip must not exceed to 15 characters")]
        public string Trip { get; set; }


        [Range(1, 999999999, ErrorMessage = "Supplier/Customer is required ")]
        public long ClientId { get; set; }


        [Range(1, 999999999, ErrorMessage = "Raw Material/Product is required ")]
        public long CommodityId { get; set; }

        public long CategoryId { get; set; }


        [Range(1, 999999999, ErrorMessage = "Bale Type is required ")]
        public long BaleTypeId { get; set; }


        public int BaleCount { get; set; }


        [MaxLength(15, ErrorMessage = "P.O. Number length must not exceed to 15 characters")]
        public string PONum { get; set; }


        [MaxLength(15, ErrorMessage = "D.R. Number length must not exceed to 15 characters")]
        public string DRNum { get; set; }


        [MaxLength(200, ErrorMessage = "Remarks length must not exceed to 200 characters")]
        [DefaultValue("")]
        public string Remarks { get; set; }


        [DefaultValue(0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal InitialWt { get; set; }


        [DefaultValue(0)]
        public decimal GrossWt { get; set; }


        [DefaultValue(0)]
        public decimal TareWt { get; set; }


        [DefaultValue(0)]
        [NotMapped]
        public decimal NetWt { get; set; }


        [NotMapped]
        public decimal Price { get; set; }


        [DefaultValue(0)]
        public decimal MC { get; set; }


        [DefaultValue(0)]
        public decimal PM { get; set; }


        [DefaultValue(0)]
        public decimal OT { get; set; }


        [NotMapped]
        public int MCStatus { get; set; }


        public string DriverName { get; set; }


        [DefaultValue(false)]
        public bool IsOfflineIn { get; set; }


        [DefaultValue(false)]
        [NotMapped]
        public bool? IsOfflineOut { get; set; }


        [MaxLength(100, ErrorMessage = "Weigher-In I.D. length must not exceed to 100 characters")]
        public string WeigherInId { get; set; }


        [MaxLength(100, ErrorMessage = "Weigher-In I.D. length must not exceed to 100 characters")]
        [NotMapped]
        public string WeigherOutId { get; set; }


        [MaxLength(50, ErrorMessage = "Truck Origin length must not exceed to 50 characters")]
        public string PlantTruckOrigin { get; set; }


        public long? MoistureReaderId { get; set; }


        [NotMapped]
        public long SignatoryId { get; set; }


        [NotMapped]
        public long MoistureSettingsId { get; set; }


        [MaxLength(20, ErrorMessage = "Time zone out length must not exceed to 20 characters.")]
        public string TimeZoneIn { get; set; }


        [MaxLength(20, ErrorMessage = "Time zone out length must not exceed to 20 characters.")]
        public string TimeZoneOut { get; set; }

        #region PURCHASE
        public long SourceId { get; set; }

        public long SourceCategoryId { get; set; }

        [MaxLength(50, ErrorMessage = "Sub Supplier length must not exceed to 50 characters")]
        public string SubSupplierName { get; set; }
        [NotMapped]
        public List<MoistureReaderLog> MoistureReaderLogs { get; set; }
        #endregion

        #region SALES
        public long? HaulerId { get; set; }

        [DefaultValue(0)]
        public decimal PlantNetWt { get; set; }
        [DefaultValue(0)]
        public decimal PlantMC { get; set; }

        [MaxLength(200, ErrorMessage = "Seal Number length must not exceed to 200 characters.")]
        public string SealNum { get; set; }
        #endregion

        public Nullable<long> VehicleTypeId { get; set; }
        [MaxLength(25, ErrorMessage = "Vehicle Type must not exceed to 25 characters")]
        public string VehicleTypeCode { get; set; }

        public string MoistureReaderDesc { get; set; }

        public string BalingStationNum { get; set; }
        public string BalingStationCode { get; set; }
        public string BalingStationName { get; set; }

        public string CategoryDesc { get; set; }
        public string SourceName { get; set; }
        public string SourceCategoryDesc { get; set; }
        public string CommodityDesc { get; set; }
        public string ClientName { get; set; }
        public string HaulerName { get; set; }
        public string BaleTypeDesc { get; set; }
        public string WeigherInName { get; set; }
        public string WeigherOutName { get; set; }

        [NotMapped]
        public List<Bale> Bales { get; set; }

        [NotMapped]
        public string VehicleNumOld {get;set;}

        [NotMapped]
        public string ReceiptNum { get; set; }

        [NotMapped]
        public TransactionProcess TransactionProcess { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }

}
