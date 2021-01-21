using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    public interface IInvDetailsRepository<T> where T : class, new()
    {
        IQueryable<T> GetByMonth(int year, int month);
        T GetById(long id);
        T Create(T model);
        T Update(T model);
        bool Delete(T model);

        Dictionary<string, string> Validate(T model);

    }
}
