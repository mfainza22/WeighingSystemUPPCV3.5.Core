using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    [Table("SourceCategoryTargets")]
    public class SourceCategoryTarget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SourceCategoryTargetId { get; set; }

        [Range(1, 99999999999, ErrorMessage = "Source Category is required")]
        public long SourceCategoryId { get; set; }

        [DefaultValue(0)]
        [Range(1, 999999999, ErrorMessage = "Target Weight must range from {0} to {1}")]
        public int Wt { get; set; }

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
