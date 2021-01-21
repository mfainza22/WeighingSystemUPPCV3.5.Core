using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IMoistureSettingsRepository : IDbRepository<MoistureSettings>
    {
        CorrectedMoisture GetCorrectedMC(decimal mc, decimal sourceWt);


    }
}
