using System;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    public interface IInyard
    {
        long InyardId { get; set; }
        DateTime DateTimeIn { get; set; }
        DateTime? DateTimeOut { get; set; }
        string VehicleNum { get; set; }
        string Trip { get; set; }
        long BaleTypeId { get; set; }
        int BaleCount { get; set; }
        string Remarks { get; set; }
        decimal GrossWt { get; set; }
        decimal TareWt { get; set; }
        decimal NetWt { get; set; }
        decimal MC { get; set; }
        decimal PM { get; set; }
        decimal OT { get; set; }
        int MCStatus { get; set; }
        long? MoistureReaderId { get; set; }
        string DriverName { get; set; }
        bool IsOfflineIn { get; set; }
        bool IsOfflineOut { get; set; }
        string WeigherInId { get; set; }
        string WeigherOutId { get; set; }
        long SignatoryId { get; set; }
        long MoistureSettingsId { get; set; }
        Nullable<long> VehicleTypeId { get; set; }
        string TimeZoneIn { get; set; }
        string TimeZoneOut { get; set; }
        string WeigherInName { get; set; }

        public string BalingStationCode { get; set; }
        public string BalingStationName { get; set; }

    }
}
