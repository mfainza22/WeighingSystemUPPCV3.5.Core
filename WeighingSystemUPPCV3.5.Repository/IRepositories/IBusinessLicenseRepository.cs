using System.Threading.Tasks;
using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IBusinessLicenseRepository : IDbRepository<BusinessLicense>
    {
        Task<BusinessLicense> CreateAsync(BusinessLicense model);
        Task<BusinessLicense> UpdateAsync(BusinessLicense model);

        Task<bool> BulkDeleteAsync(string[] id);
    }
}
