using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("Purchases")]
    public class Purchase
    {
        [Key]
        public string ReceiptNo { get; set; }

        public string PlateNo { get; set; }

        public DateTime? DateTimeIn { get; set; }

        public DateTime? DateTimeOut { get; set; }

        public string ClientId { get; set; }

        public string MaterialId { get; set; }
        public string BalesId { get; set; }

        public decimal? NoOfBales { get; set; }

        public decimal? Factory_Wt { get; set; }

        public decimal? Gross { get; set; }

        public decimal? Tare { get; set; }

        public decimal? net { get; set; }

        public decimal? Corrected_10 { get; set; }

        public decimal? Corrected_12 { get; set; }

        public decimal? Corrected_14 { get; set; }

        public decimal? Corrected_15 { get; set; }

        public decimal? Moisture { get; set; }

        public decimal? MoistureStatus { get; set; }

        public decimal? PM { get; set; }

        public decimal? OutThrow { get; set; }

        public decimal? Price { get; set; }

        public string Remarks { get; set; }

        public string PoNo { get; set; }

        public string DrNo { get; set; }

        public string DriverName { get; set; }

        public string WeigherIn { get; set; }

        public string WeigherOut { get; set; }

        public string Processed_In { get; set; }

        public string Processed_Out { get; set; }

        public string InspectorId { get; set; }

        public decimal? NoOfPrinting { get; set; }

        public decimal? Weekday { get; set; }

        public string WeekNo { get; set; }

        public DateTime? FirstDay { get; set; }

        public DateTime? LastDay { get; set; }

        public string TypeOfTrip { get; set; }

        public decimal? Plant_NetWt { get; set; }

        public decimal? Plant_Moisture { get; set; }


        public string Editing_Reasons { get; set; }

        public string Skey { get; set; }

        public string Mkey { get; set; }

        public string SubCat { get; set; }


        public string CategoryId { get; set; }


        public string SubSupplier { get; set; }

        public decimal? DDay { get; set; }


        public string DYR { get; set; }


        public string DMNT { get; set; }


        public string SelectedSourceCat { get; set; }


        public string tz_in { get; set; }


        public string tz_out { get; set; }

        public int? rp_ctr { get; set; }

        public byte? photo1 { get; set; }

        public byte? photo2 { get; set; }

        public decimal? MC1 { get; set; }

        public DateTime? MC1DT { get; set; }

        public decimal? MC2 { get; set; }

        public DateTime? MC2DT { get; set; }

        public decimal? MC3 { get; set; }

        public DateTime? MC3DT { get; set; }

        public decimal? MC4 { get; set; }

        public DateTime? MC4DT { get; set; }

        public decimal? MC5 { get; set; }

        public DateTime? MC5DT { get; set; }

        public decimal? MC6 { get; set; }

        public DateTime? MC6DT { get; set; }

        public decimal? MC7 { get; set; }

        public DateTime? MC7DT { get; set; }

        public decimal? MC8 { get; set; }

        public DateTime? MC8DT { get; set; }

        public decimal? MC9 { get; set; }

        public DateTime? MC9DT { get; set; }

        public decimal? MC10 { get; set; }

        public DateTime? MC10DT { get; set; }

        public decimal? MC11 { get; set; }

        public DateTime? MC11DT { get; set; }

        public decimal? MC12 { get; set; }

        public DateTime? MC12DT { get; set; }

        public decimal? MC13 { get; set; }

        public DateTime? MC13DT { get; set; }

        public decimal? MC14 { get; set; }

        public DateTime? MC14DT { get; set; }

        public decimal? MC15 { get; set; }

        public DateTime? MC15DT { get; set; }

        public decimal? MC16 { get; set; }

        public DateTime? MC16DT { get; set; }

        public decimal? MC17 { get; set; }

        public DateTime? MC17DT { get; set; }

        public decimal? MC18 { get; set; }

        public DateTime? MC18DT { get; set; }

        public decimal? MC19 { get; set; }

        public DateTime? MC19DT { get; set; }

        public decimal? MC20 { get; set; }

        public DateTime? MC20DT { get; set; }

        public decimal? MC21 { get; set; }

        public DateTime? MC21DT { get; set; }

        public decimal? MC22 { get; set; }

        public DateTime? MC22DT { get; set; }

        public decimal? MC23 { get; set; }

        public DateTime? MC23DT { get; set; }

        public decimal? MC24 { get; set; }

        public DateTime? MC24DT { get; set; }

        public decimal? MC25 { get; set; }

        public DateTime? MC25DT { get; set; }

        public decimal? MC26 { get; set; }

        public DateTime? MC26DT { get; set; }

        public decimal? MC27 { get; set; }

        public DateTime? MC27DT { get; set; }

        public decimal? MC28 { get; set; }

        public DateTime? MC28DT { get; set; }

        public decimal? MC29 { get; set; }

        public DateTime? MC29DT { get; set; }

        public decimal? MC30 { get; set; }

        public DateTime? MC30DT { get; set; }
    }
}
