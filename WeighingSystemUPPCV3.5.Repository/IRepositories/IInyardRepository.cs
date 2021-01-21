using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IInyardRepository : ITransDbRepository<Inyard>
    {
        Inyard WeighIn(Inyard Model);

        PurchaseTransaction WeighoutPurchase(Inyard Model);
        SaleTransaction WeighoutSale(Inyard Model);

    }
}
