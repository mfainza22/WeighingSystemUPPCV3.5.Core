using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("MoistureSettings")]
    public class MoistureSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MoistureSettingsId { get; set; }

        public decimal? M1 { get; set; }

        public decimal? M2 { get; set; }

        public decimal? M3 { get; set; }

        public decimal? M4 { get; set; }

        public decimal? D1 { get; set; }

        public decimal? D2 { get; set; }

        public decimal? D3 { get; set; }

        public decimal? D4 { get; set; }


        /// <summary>
        /// I1,I2,I3,I4 serve only as code
        /// </summary>
        public int I1 { get; set; }

        public int I2 { get; set; }

        public int I3 { get; set; }

        public int I4 { get; set; }

        public decimal? DeductFromPO { get; set; }

        public decimal? AllowablePM { get; set; }

        public decimal? AllowableOT { get; set; }

    }

}
