using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WeighingSystemUPPCV3_5_Repository.Models.OldDbs
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public string CustomerID { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string Addr { get; set; }

        public string ContactPerson { get; set; }

        public string ContactNo { get; set; }

        public bool Status { get; set; }
    }
}
