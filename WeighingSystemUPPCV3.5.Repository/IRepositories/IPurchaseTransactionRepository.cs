using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface IPurchaseTransactionRepository : ITransDbRepository<PurchaseTransaction>
    {
        IQueryable<PurchaseTransaction> GetByFilter(TransactionFilter parameters = null);

        /// <summary>
        /// Get Purchase record with MoistureReader logs included
        /// </summary>
        /// <param name="id"></param>
        /// <returns>PurchaseTransaction</returns> 
        PurchaseTransaction GetByIdWithMCReaderLogs(long id);

        IQueryable<PrintLog> GetPrintLogs(long transactionId);

        /// <summary>
        /// Update the price of purchase material to current price set in the material store
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns>decimal</returns> Current Price
        decimal UpdatePrice(long transactionId);

        Dictionary<string, string> Validate(PurchaseTransaction model);

        void MigrateOldDb(DateTime dtFrom, DateTime dtTo);

    }
}
