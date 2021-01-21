using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.Interfaces
{
    public interface IDbRepository<T> where T : class, new()
    {
        IQueryable<T> Get(T model = null);
        T GetById(long id);

        T GetByName(string name);

        T Create(T model);
        T Update(T model);
        bool Delete(T model);

        bool BulkDelete(string[] id);

        bool ValidateCode(T model);
        bool ValidateName(T model);


    }
}
