using WeighingSystemUPPCV3_5_Repository.Models;
using System.Data;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IReportingRepository
    {
        DataSet FillReportDataSet(ReportParameters reportParameters);

        void SetReportDaysWeekNum();

    }
}
