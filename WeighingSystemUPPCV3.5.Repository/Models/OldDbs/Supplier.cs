using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{
    [Table("Supplier")]
    public class Supplier
    {
        [Key]
        public string SupplierID { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string Addr { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNo { get; set; }

        public bool Status { get; set; }
    }
}
