using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IBeginningInvAdjRepository : IInvDetailsRepository<BeginningInvAdj>
    {
        IQueryable<BeginningInvAdjView> GetViewsByMonth(int year, int month);

        decimal GetActualWt(int year, int month, long categoryId);
    }
}
