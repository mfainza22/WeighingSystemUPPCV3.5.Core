using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{
    [Table("Hauler")]
    public class Hauler
    {
        [Key]
        public string HaulerID { get; set; }

        public string HaulerCode { get; set; }

        public string HaulerName { get; set; }

        public string Addr { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNo { get; set; }

        public bool Status { get; set; }
    }
}
