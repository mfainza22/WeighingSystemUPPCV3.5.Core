using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Sales")]
    public class Sale
    {

        [Key]
        public string ReceiptNo { get; set; }

        public string PlateNo { get; set; }

        public DateTime? DateTimeIn { get; set; }

        public DateTime? DateTimeOut { get; set; }

        public string CustomerId { get; set; }

        public string ProductId { get; set; }

        public string BalesId { get; set; }

        public decimal? NoOfBales { get; set; }

        public decimal? Gross { get; set; }

        public decimal? Tare { get; set; }

        public decimal? net { get; set; }

        public decimal? Corrected_10 { get; set; }

        public decimal? Corrected_12 { get; set; }

        public decimal? Corrected_14 { get; set; }

        public decimal? Corrected_15 { get; set; }

        public decimal? Moisture { get; set; }

        public decimal? MoistureStatus { get; set; }

        public decimal? OutThrow { get; set; }

        public decimal? PM { get; set; }


        public string WeigherIn { get; set; }


        public string WeigherOut { get; set; }


        public string Remarks { get; set; }


        public string DrNo { get; set; }


        public string PONo { get; set; }


        public string RefNo { get; set; }


        public string HaulerID { get; set; }

        public decimal? NoOfPrinting { get; set; }


        public string Weekday { get; set; }

        public decimal? WeekNo { get; set; }

        public DateTime? FirstDay { get; set; }

        public DateTime? LastDay { get; set; }


        public string Processed_In { get; set; }

        public string Processed_Out { get; set; }

        public string DriverName { get; set; }

        public string TypeOfTrip { get; set; }

        public decimal? Plant_NetWt { get; set; }

        public decimal? Plant_Moisture { get; set; }


        public string InspectorId { get; set; }


        public string Editing_Reasons { get; set; }


        public string Mkey { get; set; }


        public string Skey { get; set; }


        public string CategoryId { get; set; }


        public string SealNo { get; set; }


        public byte? Photo1 { get; set; }


        public byte? Photo2 { get; set; }

        public decimal? DDay { get; set; }


        public string TransType { get; set; }

        public bool? Returned { get; set; }


        public string DYR { get; set; }


        public string DMNT { get; set; }

        public DateTime? DTArrival { get; set; }

        public decimal? Plant_MC12 { get; set; }

        public decimal? Plant_MC10 { get; set; }

        public decimal? Plant_DayInterval { get; set; }

        public decimal? Plant_TimeInterval { get; set; }

        public decimal? Plant_DedWt { get; set; }

        public decimal? Plant_DiffNet { get; set; }

        public decimal? Plant_DiffMC12 { get; set; }

        public decimal? Plant_DiffMC10 { get; set; }

        public string Plant_Remarks { get; set; }


        public string Plant_TruckOrigin { get; set; }


        public string Plant_User { get; set; }

        public double? Plant_BaleCount { get; set; }

        public DateTime? Plant_Guard_in { get; set; }

        public DateTime? Plant_Guard_out { get; set; }

        public int? rp_ctr { get; set; }


        public string tz_in { get; set; }


        public string tz_out { get; set; }

        public string time_variance_remarks { get; set; }

        [ForeignKey("SalesId")]
        public virtual ICollection<BalesInv> BalesInv{ get; set; } 

    }
}
