using WeighingSystemUPPCV3_5_Repository.Models;
using System.Collections.Generic;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface ITransValidationRepository
    {
        Dictionary<string, string> ValidateInyard(Inyard model);
        Dictionary<string, string> ValidatePurchase(PurchaseTransaction model);
        Dictionary<string, string> ValidateSale(SaleTransaction model);

        bool ValidateClient(long clientId, string transTypeCode);
        bool ValidateCommodity(long commodityId, string transTypeCode);
        bool CustomerExists(long id);
        bool SupplierExists(long id);
        bool RawMaterialExists(long id);
        bool ProductExists(long id);
        bool HaulerExists(long id);
    }
}
