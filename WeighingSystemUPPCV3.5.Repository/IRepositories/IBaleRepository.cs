using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IBaleRepository
    {
        Bale Create(Bale model);

        Bale Update(Bale model);

        bool Delete(Bale model);

        bool BulkDelete(string[] id);

        void GenerateBaleCode(Bale model, out Bale outModel);

        int GetLastBaleNum(DateTime dt, long categoryId);

        IQueryable<Bale> Get(BaleFilter parameters = null);

        Dictionary<string, string> ValidateEntity(Bale model);

        bool ValidateCode(Bale model);

        int GetWarningBaleOverage();

        int GetDangerBaleOverage();

        void CheckAndCreateBaleOverageReminder();

        int GetInStockBaleWtTotal();

        void MigrateOldDb(DateTime dtFrom, DateTime dtTo);
    }
}
