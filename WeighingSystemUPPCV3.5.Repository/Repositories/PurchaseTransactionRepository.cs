
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SysUtility.Extensions;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class PurchaseTransactionRepository : IPurchaseTransactionRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IPrintLogRepository printLogRepository;

        public PurchaseTransactionRepository(DatabaseContext dbContext, IPrintLogRepository printLogRepository)
        {
            this.dbContext = dbContext;
            this.printLogRepository = printLogRepository;
        }

        public IQueryable<PurchaseTransaction> Get(PurchaseTransaction parameters = null)
        {
            return dbContext.PurchaseTransactions;
        }

        public PurchaseTransaction GetById(long id)
        {
            return dbContext.PurchaseTransactions.Find(id);
        }

        public PurchaseTransaction GetByReceiptNum(string receiptNum)
        {
            return dbContext.PurchaseTransactions.Where(a => a.ReceiptNum == receiptNum).FirstOrDefault();
        }


        public PurchaseTransaction Update(PurchaseTransaction model)
        {
            var entity = dbContext.PurchaseTransactions.Find(model.PurchaseId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.SupplierId = model.SupplierId;
            entity.Trip = model.Trip;
            entity.SupplierName = model.SupplierName;
            entity.RawMaterialId = model.RawMaterialId;
            entity.RawMaterialDesc = model.RawMaterialDesc;
            entity.MC = model.MC;
            entity.PM = model.PM;
            entity.OT = model.OT;
            entity.FactoryWt = model.FactoryWt;
            entity.MCStatus = model.MCStatus;
            entity.PONum = model.PONum?.ToUpper();
            entity.DRNum = model.DRNum?.ToUpper();
            entity.Remarks = model.Remarks?.ToUpper();
            entity.SubSupplierName = model.SubSupplierName?.ToUpper();
            entity.SourceId = model.SourceId;
            entity.SourceName = model.SourceName;
            entity.SourceCategoryId = model.SourceCategoryId;
            entity.DriverName = model.DriverName;
            entity.Price = model.Price;
            entity.SourceCategoryId = model.SourceCategoryId;
            entity.SourceCategoryDesc = model.SourceCategoryDesc;
            entity.MoistureReaderId = model.MoistureReaderId;
            entity.MoistureReaderLogsModified = model.MoistureReaderLogsModified;

            dbContext.PurchaseTransactions.Update(entity);

            if (model.MoistureReaderLogsModified)
            {
                dbContext.RemoveRange(dbContext.moistureReaderLogs.Where(a => a.TransactionId == model.PurchaseId));
                foreach (var moistureReaderLog in model.MoistureReaderLogs)
                {
                    moistureReaderLog.TransactionId = model.PurchaseId;
                };

                dbContext.AddRange(model.MoistureReaderLogs);
            }
            dbContext.SaveChanges();
            return entity;
        }

        public decimal UpdatePrice(long transactionId)
        {
            var purchase = dbContext.PurchaseTransactions.AsNoTracking().FirstOrDefault(a => a.PurchaseId == transactionId);
            var updatedPrice = dbContext.RawMaterials.Where(a => a.RawMaterialId == purchase.RawMaterialId).Select(a => a.Price).FirstOrDefault();
            dbContext.Database.ExecuteSqlRaw($"Update PurchaseTransactions set Price = {updatedPrice} where PurchaseId = {updatedPrice}");
            return updatedPrice;
        }

        public bool Delete(PurchaseTransaction model)
        {
                dbContext.PurchaseTransactions.Remove(model);
                dbContext.RemoveRange(dbContext.moistureReaderLogs.Where(a => a.TransactionId == model.PurchaseId));
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.PurchaseTransactions.Where(a => id.Contains(a.PurchaseId.ToString()));

                dbContext.PurchaseTransactions.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<PurchaseTransaction> GetByFilter(TransactionFilter parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            return dbContext.PurchaseTransactions.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public SqlRawParameter GetSqlRawParameter(TransactionFilter parameters = null)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM PurchaseTransactions");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.TransactionId.IsNullOrZero())
            {
                sqlParams.Add(new SqlParameter(nameof(PurchaseTransaction.PurchaseId).Parametarize(), parameters.TransactionId));
                whereClauses.Add($"{nameof(PurchaseTransaction.PurchaseId)} = {nameof(PurchaseTransaction.PurchaseId).Parametarize()}");
            }

            if (!parameters.ReceiptNum.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(PurchaseTransaction.ReceiptNum).Parametarize(), parameters.ReceiptNum));
                whereClauses.Add($"{nameof(PurchaseTransaction.ReceiptNum)} = {nameof(PurchaseTransaction.ReceiptNum).Parametarize()}");
            }
            if (parameters.DTFrom.HasValue)
            {
                parameters.DTFrom = parameters.DTFrom.Value.Date + new TimeSpan(0, 0, 0);
                parameters.DTTo = parameters.DTTo.Value.Date + new TimeSpan(23, 59, 59);
                sqlParams.Add(new SqlParameter(nameof(parameters.DTFrom).Parametarize(), parameters.DTFrom));
                sqlParams.Add(new SqlParameter(nameof(parameters.DTTo).Parametarize(), parameters.DTTo));
                whereClauses.Add($"{nameof(PurchaseTransaction.DateTimeOut)} between {nameof(parameters.DTFrom).Parametarize()} and {nameof(parameters.DTTo).Parametarize()}");
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public SqlRawParameter GetSqlRawParameter(PurchaseTransaction parameters) => throw new NotImplementedException();

        public Dictionary<string, string> Validate(PurchaseTransaction model)
        {
            throw new NotImplementedException();
        }

        public DataSet PrintReceipt(PrintReceiptModel model)
        {
            if (model.IsReprinted) printLogRepository.Create(model);

            var serverDataSet = new DataSet();
            serverDataSet.EnforceConstraints = false;
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.PurchaseTransactions)));
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.moistureReaderLogs)));

            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                var query = string.Empty;
                var sa = new SqlDataAdapter();

                query = dbContext.PurchaseTransactions
                    .Where(a => a.PurchaseId == model.TransactionId).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.PurchaseTransactions));
                sa.Dispose();

                query = dbContext.moistureReaderLogs.Where(a => a.TransactionId == model.TransactionId).Take(1).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.moistureReaderLogs));
                sa.Dispose();

                query = null;
            }

            return serverDataSet;
        }

        public IQueryable<PrintLog> GetPrintLogs(long transactionId)
        {
            return printLogRepository.Get(transactionId);
        }

        public PurchaseTransaction GetByIdWithMCReaderLogs(long id)
        {
            return dbContext.PurchaseTransactions.Include(a => a.MoistureReaderLogs).AsNoTracking().FirstOrDefault(a => a.PurchaseId == id);
        }

        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
            var baleTypes = dbContext.BaleTypes.AsNoTracking().ToList();
            var suppliers = dbContext.Suppliers.AsNoTracking().ToList();
            var rawMaterials = dbContext.RawMaterials.AsNoTracking().ToList();
            var weighers = dbContext.UserAccounts.AsNoTracking().ToList();
            var sourceCategories = dbContext.SourceCategories.AsNoTracking().ToList();
            var sources = dbContext.Sources.AsNoTracking().ToList();

            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var purchases = dbContext.Purchases.Where(a => a.DateTimeIn >= dtFrom && a.DateTimeIn <= dtTo).OrderBy(a=>a.ReceiptNo).AsNoTracking().ToList();
            foreach (var purchase in purchases)
            {
                var exist = dbContext.PurchaseTransactions.Where(a => a.VehicleNum == purchase.PlateNo && a.DateTimeIn == purchase.DateTimeIn).AsNoTracking().Count();
                if (exist > 0) continue;

                var purchaseTrans = new PurchaseTransaction();
                purchaseTrans.ReceiptNum = purchase.ReceiptNo.Trim();
                purchaseTrans.DateTimeIn = purchase.DateTimeIn.Value;
                purchaseTrans.DateTimeOut = purchase.DateTimeOut;
                purchaseTrans.VehicleNum = purchase.PlateNo;
                var supplier = suppliers.FirstOrDefault(a => a.SupplierIdOld == purchase.ClientId);
                if (supplier != null)
                {
                    purchaseTrans.SupplierId = supplier.SupplierId;
                    purchaseTrans.SupplierName = supplier.SupplierName;
                }
                var material = rawMaterials.FirstOrDefault(a => a.RawMaterialIdOld == purchase.MaterialId);
                if (material != null)
                {
                    purchaseTrans.RawMaterialId = material.RawMaterialId;
                    purchaseTrans.RawMaterialDesc = material.RawMaterialDesc;
                    purchaseTrans.CategoryId = material.CategoryId;
                }
                purchaseTrans.Price = purchase.Price ?? 0;
                var baleType = baleTypes.FirstOrDefault(a => a.BaleTypeIdOld == purchase.BalesId);
                if (baleType != null)
                {
                    purchaseTrans.BaleTypeId = baleType.BaleTypeId;
                    purchaseTrans.BaleTypeDesc = baleType.BaleTypeDesc;
                }
                purchaseTrans.BaleCount = (int)purchase.NoOfBales;
                purchaseTrans.FactoryWt = (int)purchase.Factory_Wt;
                purchaseTrans.GrossWt = purchase.Gross ?? 0;
                purchaseTrans.TareWt = purchase.Tare ?? 0;
                purchaseTrans.NetWt = purchase.net ?? 0;
                purchaseTrans.Corrected10 = purchase.Corrected_10 ?? 0;
                purchaseTrans.Corrected12 = purchase.Corrected_12 ?? 0;
                purchaseTrans.Corrected14 = purchase.Corrected_14 ?? 0;
                purchaseTrans.Corrected15 = purchase.Corrected_15 ?? 0;
                purchaseTrans.MC = purchase.Moisture ?? 0;
                purchaseTrans.MCStatus = Convert.ToInt32(purchase.MoistureStatus ?? 0);
                purchaseTrans.PM = purchase.PM ?? 0;
                purchaseTrans.OT = purchase.OutThrow ?? 0;
                purchaseTrans.Price = purchase.Price ?? 0;
                purchaseTrans.Remarks = purchase.Remarks;
                purchaseTrans.PONum = purchase.PoNo;
                purchaseTrans.DRNum = purchase.DrNo; ;
                purchaseTrans.DriverName = purchase.DriverName;
                var weigher = weighers.FirstOrDefault(a => a.UserAccountIdOld == purchase.WeigherIn.Trim());
                if (weigher != null)
                {
                    purchaseTrans.WeigherInId = weigher.UserAccountId;
                    purchaseTrans.WeigherInName = weigher.FullName;
                }
                weigher = weighers.FirstOrDefault(a => a.UserAccountIdOld == purchase.WeigherOut.Trim());
                if (weigher != null)
                {
                    purchaseTrans.WeigherOutId = weigher.UserAccountId;
                    purchaseTrans.WeigherOutName = weigher.FullName;
                }
                purchaseTrans.IsOfflineIn = purchase.Processed_In == "OFFLINE";
                purchaseTrans.IsOfflineOut = purchase.Processed_Out == "OFFLINE";
                purchaseTrans.PrintCount = (int)(purchase.NoOfPrinting ?? 0);
                purchaseTrans.WeekDay = (int)purchase.Weekday;
                purchaseTrans.WeekNum = Convert.ToInt32(purchase.WeekNo);
                purchaseTrans.FirstDay = purchase.FirstDay;
                purchaseTrans.LastDay = purchase.LastDay;
                purchaseTrans.Trip = purchase.TypeOfTrip;
                purchaseTrans.MoistureSettingsId = 1;
                purchaseTrans.SignatoryId = 1;
                var sourceCat = sourceCategories.FirstOrDefault(a => a.Description == purchase.SelectedSourceCat);
                if (sourceCat != null)
                {
                    purchaseTrans.SourceCategoryId = sourceCat.SourceCategoryId;
                    purchaseTrans.SourceCategoryDesc = sourceCat.Description;
                }
                var source = sources.FirstOrDefault(a => a.SourceDesc == purchase.SubCat);
                if (source != null)
                {
                    purchaseTrans.SourceId = source.SourceId;
                    purchaseTrans.SourceName = source.SourceDesc;
                }
                purchaseTrans.SubSupplierName = purchase.SubSupplier;
                purchaseTrans.PONum = purchase.PoNo;
                purchaseTrans.DRNum = purchase.DrNo;
                purchaseTrans.TimeZoneIn = purchase.tz_in ?? purchase.DateTimeIn.GetTimeZone();
                purchaseTrans.TimeZoneOut = purchase.tz_out ?? purchase.DateTimeOut.GetTimeZone();

                dbContext.PurchaseTransactions.Add(purchaseTrans);
                dbContext.SaveChanges();

                var mcLogs = new List<MoistureReaderLog>();
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 1, purchase.MC1DT, purchase.MC1));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 2, purchase.MC2DT, purchase.MC2));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 3, purchase.MC3DT, purchase.MC3));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 4, purchase.MC4DT, purchase.MC4));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 5, purchase.MC5DT, purchase.MC5));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 6, purchase.MC6DT, purchase.MC6));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 7, purchase.MC7DT, purchase.MC7));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 8, purchase.MC8DT, purchase.MC8));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 9, purchase.MC9DT, purchase.MC9));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 10, purchase.MC10DT, purchase.MC10));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 11, purchase.MC11DT, purchase.MC11));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 12, purchase.MC12DT, purchase.MC12));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 13, purchase.MC13DT, purchase.MC13));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 14, purchase.MC14DT, purchase.MC14));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 15, purchase.MC15DT, purchase.MC15));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 16, purchase.MC16DT, purchase.MC16));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 17, purchase.MC17DT, purchase.MC17));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 18, purchase.MC18DT, purchase.MC18));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 19, purchase.MC19DT, purchase.MC19));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 20, purchase.MC20DT, purchase.MC20));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 21, purchase.MC21DT, purchase.MC21));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 22, purchase.MC22DT, purchase.MC22));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 23, purchase.MC23DT, purchase.MC23));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 24, purchase.MC24DT, purchase.MC24));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 25, purchase.MC25DT, purchase.MC25));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 26, purchase.MC26DT, purchase.MC26));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 27, purchase.MC27DT, purchase.MC27));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 28, purchase.MC28DT, purchase.MC28));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 29, purchase.MC29DT, purchase.MC29));
                mcLogs.Add(createMcLog(purchaseTrans.PurchaseId, 30, purchase.MC30DT, purchase.MC30));
                dbContext.moistureReaderLogs.AddRange(mcLogs);
                dbContext.SaveChanges();
            }
        }

        private MoistureReaderLog createMcLog(long transId, int logNUm, DateTime? dt, decimal? mc)
        {
            return new MoistureReaderLog()
            {
                TransactionId = transId,
                LogNum = logNUm,
                DTLog = dt ?? DateTime.Now,
                MC = mc ?? 0
            };
        }
    }
}
