using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public class PrintLogRepository : IPrintLogRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IConfiguration configuration;

        public PrintLogRepository(DatabaseContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
        }

        public PrintLog Create(PrintReceiptModel model)
        {
            var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var table = model.TransactionTypeCode == "I" ? "PurchaseTransactions" : "SaleTransactions";
                var colName = model.TransactionTypeCode == "I" ? "PurchaseId" : "SaleId";
                var qry = $"UPDATE {table} SET {nameof(PurchaseTransaction.PrintCount)} = " +
                $"{nameof(PurchaseTransaction.PrintCount)}+1 WHERE {colName} ={model.TransactionId}";
                dbContext.Database.ExecuteSqlRaw(qry);

                var printLog = new PrintLog()
                {
                    DTPrinted = DateTime.Now,
                    TransactionId = model.TransactionId,
                    TransactionTypeCode = model.TransactionTypeCode,
                    PrintReasons = model.ReprintRemarks,
                    ReceiptNum = model.ReceiptNum,
                    UserAccountId = model.UserAccoountId,
                    UserAccountName = model.UserFullName
                };
                dbContext.PrintLogs.Add(printLog);
                dbContext.SaveChanges();

                transaction.Commit();
                return printLog;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        public IQueryable<PrintLog> Get(PrintLog parameters = null)
        {
            if (parameters == null) return new List<PrintLog>().AsQueryable();

            return dbContext.PrintLogs.Where(a =>
            (a.TransactionId == parameters.TransactionId &&
            (a.TransactionTypeCode == parameters.TransactionTypeCode)));
        }
    }
}
