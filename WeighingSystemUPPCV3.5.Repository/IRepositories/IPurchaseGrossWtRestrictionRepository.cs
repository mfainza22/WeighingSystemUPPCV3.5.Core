using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IPurchaseGrossWtRestrictionRepository
    {

        PurchaseGrossWtRestriction Create(PurchaseGrossWtRestriction model);

        PurchaseGrossWtRestriction Update(PurchaseGrossWtRestriction oldModel, PurchaseGrossWtRestriction newModel);

        PurchaseGrossWtRestriction CheckRestriction(PurchaseGrossWtRestriction model);

    }
}
