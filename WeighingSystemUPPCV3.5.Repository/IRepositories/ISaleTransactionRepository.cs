using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public interface ISaleTransactionRepository : ITransDbRepository<SaleTransaction>
    {
        IQueryable<SaleTransaction> GetByFilter(TransactionFilter parameters = null);

        IQueryable<SaleTransaction> GetUnreturnedVehicles();

        SqlRawParameter GetSqlRawParameter(TransactionFilter parameters = null);

        Dictionary<string, string> Validate(SaleTransaction model);

        SaleTransaction GetById(long id,bool includeBales = false);

        decimal UpdateMCStatus(long id, decimal mcStatus);

        List<SaleBale> UpdateBales(long id,List<SaleBale> newSaleBales);

        void MigrateOldDb(DateTime dtFrom, DateTime dtTo);

        IQueryable<PrintLog> GetPrintLogs(long transactionId);
    }
}
