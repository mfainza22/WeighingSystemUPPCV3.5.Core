using System;
using System.Collections.Generic;

namespace WeighingSystemUPPCV3_5_Repository.Models
{
    public class MoistureReaderJob
    {
        public long JobNum { get; set; }

        public DateTime DTStart { get; set; }

        public DateTime DTEnd { get; set; }

        public Decimal Average { get; set; }

        public List<MoistureReaderLog> Logs { get; set; }




    }
}
