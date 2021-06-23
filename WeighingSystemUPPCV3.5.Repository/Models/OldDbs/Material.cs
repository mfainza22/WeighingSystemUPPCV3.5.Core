using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{
    [Table("Material")]
    public class Material
    {
        [Key]
        public string MaterialID { get; set; }

        public string MaterialDesc { get; set; }

        public decimal? Price { get; set; }

        public bool? Status { get; set; }

        public string MaterialCode {get;set;}

        public string MateCat { get; set; }

        public string CurrentPO { get; set; }

        public long? SeqNo { get; set; }

        public long? RawMaterialId { get; set; }
        public string RawMaterialDesc{ get; set; }

    }
}
