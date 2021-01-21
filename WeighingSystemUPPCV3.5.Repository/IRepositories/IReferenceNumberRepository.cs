using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IReferenceNumberRepository : IDbRepository<ReferenceNumber>
    {

        bool ValidateInyardNum(ReferenceNumber model);

        bool ValidatePurchaseReceiptNum(ReferenceNumber model);

        bool ValidateSalesReceiptNum(ReferenceNumber model);
    }
}
