using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrderView> CreateAsync(PurchaseOrder model);

        Task<PurchaseOrderView> UpdateAsync(PurchaseOrder model);

        bool Delete(PurchaseOrder model);

        Task<bool> BulkDelete(string[] id);

        IQueryable<PurchaseOrder> Get(PurchaseOrder parameters = null);

        IQueryable<PurchaseOrderView> GetView(PurchaseOrder parameters = null);

        PurchaseOrderView ValidatePO(PurchaseOrder parameters = null);

        Dictionary<string, string> ValidateEntity(PurchaseOrder model);

        bool ValidatePONum(PurchaseOrder model);

        void MigrateOldDb(DateTime dtFrom, DateTime dtTo);

    }
}
