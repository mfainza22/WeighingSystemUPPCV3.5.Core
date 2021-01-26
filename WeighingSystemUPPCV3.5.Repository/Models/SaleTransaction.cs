
using WeighingSystemUPPCV3_5_Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WeighingSystemUPPCV3_5_Repository.Models
{

    [Table("SaleTransactions")]

    public class SaleTransaction : IInyard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SaleId { get; set; }


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


        public long CustomerId { get; set; }

        public long HaulerId { get; set; }


        [Range(1, 999999999, ErrorMessage = "Product is required.")]
        public long ProductId
        {
            get; set;
        }

        [Range(1, 999999999, ErrorMessage = "Category is required.")]
        public long CategoryId { get; set; }


        [Range(1, 999999999, ErrorMessage = "Bale Type is required.")]
        public long BaleTypeId { get; set; }

        [DefaultValue(0)]
        public int BaleCount { get; set; }

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


        public decimal Price { get; set; }


        [Required(ErrorMessage = " Driver Name is required ", AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "Driver Name length must not exceed to 50 characters")]
        public string DriverName { get; set; }


        [MaxLength(200, ErrorMessage = "Seal Number length must not exceed to 200 characters")]
        public string SealNum { get; set; }


        [Required(ErrorMessage = " Remarks is required ", AllowEmptyStrings = false)]
        [MaxLength(200, ErrorMessage = "Remarks length must not exceed to 200 characters")]
        public string Remarks { get; set; }

        public long? MoistureReaderId { get; set; }


        [MaxLength(20, ErrorMessage = "TimeZoneIn length must not exceed to 20 characters")]
        public string TimeZoneIn { get; set; }



        [MaxLength(20, ErrorMessage = "TimeZoneOut length must not exceed to 20 characters")]
        public string TimeZoneOut { get; set; }


        [DefaultValue(false)]
        public bool IsOfflineIn { get; set; }


        [DefaultValue(false)]
        public bool IsOfflineOut { get; set; }


        [MaxLength(100, ErrorMessage = "WeigherInId length must not exceed to 100 characters")]
        public string WeigherInId { get; set; }


        [MaxLength(100, ErrorMessage = "WeigherOutId length must not exceed to 100 characters")]
        public string WeigherOutId { get; set; }


        [DefaultValue(0)]
        public int WeekDay { get; set; }


        [DefaultValue(0)]
        public int WeekNum { get; set; }


        public DateTime FirstDay { get; set; }


        public DateTime LastDay { get; set; }


        [DefaultValue(1)]
        public long SignatoryId { get; set; }


        [DefaultValue(1)]
        public long MoistureSettingsId { get; set; }


        [DefaultValue(0)]
        public int PrintCount { get; set; }

        public Nullable<long> VehicleTypeId { get; set; }

        [MaxLength(25, ErrorMessage = "Vehicle Type must not exceed to 25 characters")]
        public string VehicleTypeCode { get; set; }


        [MaxLength(50, ErrorMessage = "BaleTypeDesc length must not exceed to 50 characters")]
        public string BaleTypeDesc { get; set; }


        [MaxLength(50, ErrorMessage = "CustomerName length must not exceed to 50 characters")]
        public string CustomerName { get; set; }


        [MaxLength(50, ErrorMessage = "ProductDesc length must not exceed to 50 characters")]
        public string ProductDesc { get; set; }

        [MaxLength(50, ErrorMessage = "CategoryDesc length must not exceed to 50 characters")]
        public string CategoryDesc { get; set; }


        [MaxLength(50, ErrorMessage = "Hauler Name length must not exceed to 50 characters")]
        public string HaulerName { get; set; }


        [MaxLength(50, ErrorMessage = "WeigherInName length must not exceed to 50 characters")]
        public string WeigherInName { get; set; }


        [MaxLength(50, ErrorMessage = "WeigherOutName length must not exceed to 50 characters")]
        public string WeigherOutName { get; set; }

        [MaxLength(50, ErrorMessage = "Moisture Reader Description length must not exceed to 50 characters")]
        public string MoistureReaderDesc { get; set; }

        [MaxLength(50, ErrorMessage = "Baling Station Code length must not exceed to 50 characters")]
        public string BalingStationCode { get; set; }

        [MaxLength(50, ErrorMessage = "Baling Station Name length must not exceed to 50 characters")]
        public string BalingStationName { get; set; }

        [ForeignKey("SaleId")]
        public virtual ICollection<Bale> Bales { get; set; }

        [NotMapped]
        public long InyardId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateOut { get; set; }

        public virtual ReturnedVehicle ReturnedVehicle { get; set; }

    }

}
