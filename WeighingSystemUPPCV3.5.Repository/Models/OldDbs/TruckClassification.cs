using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{
    [Table("TruckClassification")]
    public class TruckClassification
    {
        [Key]
        public string TruckCode { get; set; }

        public string Description { get; set; }

        public decimal? MinError { get; set; }

        public decimal? MaxError { get; set; }

    }
}
