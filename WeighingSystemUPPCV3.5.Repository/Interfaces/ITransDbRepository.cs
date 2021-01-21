using WeighingSystemUPPCV3_5_Repository.Models;
using System.Data;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    public interface ITransDbRepository<T> where T : class, new()
    {
        IQueryable<T> Get(T model = null);
        T GetById(long id);
        T GetByReceiptNum(string id);
        T Update(T model);
        bool Delete(T model);
        bool BulkDelete(string[] id);

        /// <summary>
        /// Not Really printing,
        /// Only create print logs
        /// </summary>
        /// <param name="model"></param>
        DataSet PrintReceipt(PrintReceiptModel model);

    }
}
