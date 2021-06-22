using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{

    [Table("Beginning_Adjustment")]
    public class Beginning_Adjustment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SeqNo { get; set; }

        public string CatId { get; set; }

        public string AdjYear { get; set; }

        public string AdjMonth { get; set; }

        public decimal AdjustedVal { get; set; }

        public decimal Adjusted10 { get; set; }

        public decimal AdjustedBales { get; set; }
    }
}
