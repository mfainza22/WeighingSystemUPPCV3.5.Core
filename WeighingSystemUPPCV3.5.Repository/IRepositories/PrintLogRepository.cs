using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace WeighingSystemUPPCV3_5_Repository.IRepositories
{
    public class PrintLogRepository : IPrintLogRepository
    {
        private readonly DatabaseContext dbContext;

        public PrintLogRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public PrintLog Create(PrintReceiptModel model)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var qry = $"UPDATE PurchaseTransactions SET {nameof(PurchaseTransaction.PrintCount)} = " +
                $"{nameof(PurchaseTransaction.PrintCount)}+1 WHERE PurchaseId ={model.TransactionId}";
                dbContext.Database.ExecuteSqlRaw(qry);

                var printLog = new PrintLog()
                {
                    DTPrinted = DateTime.Now,
                    TransactionId = model.TransactionId,
                    TransactionTypeCode = model.TransactionTypeCode,
                    PrintReasons = model.ReprintRemarks,
                    ReceiptNum = model.ReceiptNum
                };
                dbContext.PrintLogs.Add(printLog);
                dbContext.SaveChanges();
                //throw new Exception("TRY");

                transaction.Commit();
                return printLog;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<PrintLog> Get(long transactionId)
        {
            return dbContext.PrintLogs.Where(a => a.TransactionId == transactionId);
        }
    }
}
