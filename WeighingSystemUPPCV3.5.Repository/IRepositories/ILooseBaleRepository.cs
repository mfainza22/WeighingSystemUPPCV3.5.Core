using System;
using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface ILooseBaleRepository : IInvDetailsRepository<LooseBale>
    {
        void MigrateOldDb(DateTime dtFrom, DateTime dtTo);
    }
}
