using System;

namespace WeighingSystemUPPCV3_5_Repository.ViewModels
{
    public class InyardListViewModel
    {

        public long InyardId { get; set; }

        public string InyardNum { get; set; }

        public DateTime DateTimeIn { get; set; }

        public string VehicleNum { get; set; }

        public string ClientName { get; set; }

        public string CommodityDesc { get; set; }

        public decimal InitialWt { get; set; }

        public string WeigherInName { get; set; }
    }
}
