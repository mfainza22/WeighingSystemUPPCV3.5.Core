using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface ICalibrationRepository
    {

        Calibration Create(Calibration model);


        Calibration Update(Calibration model);


        bool Delete(Calibration model);


        bool BulkDelete(string[] id);


        IQueryable<Calibration> Get(Calibration model = null);


        Calibration Confirm(Calibration model);

        Calibration UpdateLastLog(Calibration model);


        CalibrationLog GetLastLog(long id);

    }
}
