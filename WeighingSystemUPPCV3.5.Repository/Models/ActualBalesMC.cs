using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("ActualBalesMCs")]
    public class ActualBalesMC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ActualBalesMCId { get; set; }

        [Range(1, 99999999999, ErrorMessage = "Category is required")]
        public long CategoryId { get; set; }

        [DefaultValue(0D)]
        [Range(1, 100, ErrorMessage = "Moisture Content must be range from 1 to 100")]
        public decimal MC { get; set; }

        public DateTime DT { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? DYear { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? DMonth { get; set; }

        public int WeekDay { get; set; }

        public int WeekNum { get; set; }

        public DateTime FirstDay { get; set; }

        public DateTime LastDay { get; set; }


    }
}
