using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IBalingStationRepository : IDbRepository<BalingStation>
    {
        BalingStation GetByNum(string balingStationNum);

        BalingStation GetByCode(string balingStationCode);

        BalingStation GetSelected();

        BalingStation RestrictReceiving(long balingStationId);

        BalingStation UnRestrictReceiving(long balingStationId);

        decimal GetWarehouseSpaceStatus();

        void CheckAndCreateStockStatusReminder();

        string BackupDatabase(bool shirnkDatabase = false);

        void UploadBackupFile(string fileName);

    }
}
