using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IPrintLogRepository
    {
        PrintLog Create(PrintReceiptModel model);

        IQueryable<PrintLog> Get(PrintLog parameters = null);
    }
}
