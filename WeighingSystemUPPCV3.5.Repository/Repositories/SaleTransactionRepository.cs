
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class SaleTransactionRepository : ISaleTransactionRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IPrintLogRepository printLogRepository;
        private readonly IBaleRepository baleRepository;
        private readonly IReturnedVehicleRepository returnedVehicleRepository;
        private readonly ILogger<SaleTransactionRepository> logger;

        public SaleTransactionRepository(DatabaseContext dbContext,
            IPrintLogRepository printLogRepository,
            IBaleRepository baleRepository, 
            IReturnedVehicleRepository returnedVehicleRepository,
            ILogger<SaleTransactionRepository> logger)
        {
            this.dbContext = dbContext;
            this.printLogRepository = printLogRepository;
            this.baleRepository = baleRepository;
            this.returnedVehicleRepository = returnedVehicleRepository;
            this.logger = logger;
        }

        public IQueryable<SaleTransaction> Get(SaleTransaction parameters = null)
        {
            return dbContext.SaleTransactions;
        }

        public SaleTransaction GetById(long id)
        {
            return dbContext.SaleTransactions.AsNoTracking().Where(a => a.SaleId == id).FirstOrDefault();
        }

        public SaleTransaction GetByReceiptNum(string receiptNum)
        {
            return dbContext.SaleTransactions.AsNoTracking().Where(a => a.ReceiptNum == receiptNum).FirstOrDefault();
        }

        public SaleTransaction Update(SaleTransaction model)
        {
            var entity = dbContext.SaleTransactions.AsNoTracking().FirstOrDefault(a => a.SaleId == model.SaleId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            entity.CustomerId = model.CustomerId;
            entity.CustomerName = model.CustomerName;
            entity.ProductId = model.ProductId;
            entity.ProductDesc = model.ProductDesc;
            entity.HaulerId = model.HaulerId;
            entity.HaulerName = model.HaulerName;
            entity.SealNum = model.SealNum;

            dbContext.SaleTransactions.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool Delete(SaleTransaction model)
        {
                dbContext.SaleTransactions.Remove(model);
                var qry = $"UPDATE Bales SET ";
                qry += $"{nameof(Bale.SaleId)}=${model.SaleId}, ";
                qry += $"{nameof(Bale.DTDelivered)}=null ";
                qry += $"WHERE SaleId = {model.SaleId}";
                dbContext.Database.ExecuteSqlRaw(qry);
                dbContext.SaveChanges();
                return true;
        }

        public bool BulkDelete(string[] id)
        {
                if (id == null) return false;
                if (id.Length == 0) return false;

                var entitiesToDelete = dbContext.SaleTransactions.Where(a => id.Contains(a.SaleId.ToString()));

                dbContext.SaleTransactions.RemoveRange(entitiesToDelete);
                dbContext.SaveChanges();
                return true;
        }

        public IQueryable<SaleTransaction> GetByFilter(TransactionFilter parameters = null)
        {
            var sqlRawParams = GetSqlRawParameter(parameters);
            return dbContext.SaleTransactions.FromSqlRaw(sqlRawParams.SqlQuery, sqlRawParams.SqlParameters.ToArray()).AsNoTracking();
        }

        public IQueryable<SaleTransaction> GetUnreturnedVehicles()
        {
            var parameters = new TransactionFilter()
            {
                Returned = false
            };
            return GetByFilter(parameters);
        }

        public SqlRawParameter GetSqlRawParameter(TransactionFilter parameters = null)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM SaleTransactions");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.TransactionId.IsNullOrZero())
            {
                sqlParams.Add(new SqlParameter(nameof(SaleTransaction.SaleId).Parametarize(), parameters.TransactionId));
                whereClauses.Add($"{nameof(SaleTransaction.SaleId)} = {nameof(SaleTransaction.SaleId).Parametarize()}");
            }

            if (!parameters.ReceiptNum.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(SaleTransaction.ReceiptNum).Parametarize(), parameters.ReceiptNum));
                whereClauses.Add($"{nameof(SaleTransaction.ReceiptNum)} = {nameof(SaleTransaction.ReceiptNum).Parametarize()}");
            }
            if (parameters.DTFrom.HasValue)
            {
                parameters.DTFrom = parameters.DTFrom.Value.Date + new TimeSpan(0, 0, 0);
                parameters.DTTo = parameters.DTTo.Value.Date + new TimeSpan(23, 59, 59);
                sqlParams.Add(new SqlParameter(nameof(parameters.DTFrom).Parametarize(), parameters.DTFrom));
                sqlParams.Add(new SqlParameter(nameof(parameters.DTTo).Parametarize(), parameters.DTTo));
                whereClauses.Add($"{nameof(SaleTransaction.DateTimeOut)} between {nameof(parameters.DTFrom).Parametarize()} and {nameof(parameters.DTTo).Parametarize()}");
            }

            if (parameters.Returned.HasValue)
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.Returned).Parametarize(), parameters.Returned));
                whereClauses.Add($"{nameof(SaleTransaction.Returned)} = {nameof(parameters.Returned).Parametarize()}");
            }

            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public SqlRawParameter GetSqlRawParameter(SaleTransaction parameters)
        {
            throw new NotImplementedException();
        }


        public Dictionary<string, string> Validate(SaleTransaction model)
        {
            throw new NotImplementedException();
        }

        public DataSet PrintReceipt(PrintReceiptModel model)
        {
            var qry = $"UPDATE SaleTransactions SET {nameof(PurchaseTransaction.PrintCount)} = " +
                $"{nameof(PurchaseTransaction.PrintCount).Parametarize()}+1 WHERE SaleTransactionId ={model.TransactionId}";
            dbContext.Database.ExecuteSqlRaw(qry);

            printLogRepository.Create(model);

            var serverDataSet = new DataSet();
            serverDataSet.EnforceConstraints = false;
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.SaleTransactions)));

            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                var query = string.Empty;
                var sa = new SqlDataAdapter();

                query = dbContext.SaleTransactions
              .Where(a => a.SaleId == model.TransactionId).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.SaleTransactions));
                sa.Dispose();

                query = null;
            }

            return serverDataSet;
        }

        public SaleTransaction GetByIdWithBales(long id)
        {
            return dbContext.SaleTransactions.Include(a => a.Bales).AsNoTracking().FirstOrDefault(a => a.SaleId == id);
        }

        public decimal UpdateMCStatus(long id, decimal mcStatus)
        {
            var sqlParams = new List<SqlParameter>();
            var sqlQuery = new StringBuilder();

            sqlParams.Add(new SqlParameter(nameof(SaleTransaction.SaleId).Parameterize(), id));
            sqlParams.Add(new SqlParameter(nameof(SaleTransaction.MCStatus).Parameterize(), mcStatus));

            sqlQuery.AppendLine($"UPDATE SaleTransactions set {nameof(SaleTransaction.MCStatus)} = {nameof(SaleTransaction.MCStatus).Parameterize()}");
            sqlQuery.AppendLine($" WHERE {nameof(SaleTransaction.SaleId)}  = {nameof(SaleTransaction.SaleId).Parameterize()} ");

            dbContext.Database.ExecuteSqlRaw(sqlQuery.ToString(), sqlParams.ToArray());

            return mcStatus;
        }

        public List<Bale> UpdateBales(SaleTransaction model)
        {
            var baleFilter = new BaleFilter();
            baleFilter.SaleId = model.SaleId;

            var origBales = baleRepository.Get(baleFilter);

            var deletedBale = origBales.Select(a => a.BaleId).ToArray().Except(model.Bales.ToList().Select(a => a.BaleId).ToArray());

            var addedBales = model.Bales.Select(a => a.BaleId).ToArray().Except(origBales.ToList().Select(a => a.BaleId).ToArray());

            var newBaleSQLQuery = new StringBuilder();
            if (addedBales.ToList().Count() > 0)
            {
                newBaleSQLQuery.AppendLine($"Update Bales set SaleId = '{model.SaleId}', DTDelivered = '{model.DateTimeOut}'");
                newBaleSQLQuery.AppendLine($"WHERE BaleId in ({String.Join(",", addedBales.ToArray())})");
            }

            var deletedBalesQry = new StringBuilder();
            if (deletedBale.ToList().Count() > 0)
            {
                deletedBalesQry.AppendLine($"Update Bales set SaleId = null, DTDelivered = null");
                deletedBalesQry.AppendLine($"WHERE BaleId in ({String.Join(",", deletedBale.ToArray())})");
            }

            if (addedBales.ToList().Count > 0 || deletedBale.ToList().Count() > 0)
            {
                dbContext.Database.ExecuteSqlRaw(newBaleSQLQuery.AppendLine(deletedBalesQry.ToString()).ToString());
            }

            return origBales.ToList();
        }

        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
            var baleTypes = dbContext.BaleTypes.AsNoTracking().ToList();
            var customers = dbContext.Customers.AsNoTracking().ToList();
            var products = dbContext.Products.Include(a => a.Category).AsNoTracking().ToList();
            var haulers = dbContext.Haulers.AsNoTracking().ToList();
            var weighers = dbContext.UserAccounts.AsNoTracking().ToList();

            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var sales = dbContext.Sales.Where(a => a.DateTimeIn >= dtFrom && a.DateTimeIn <= dtTo).AsNoTracking().ToList();
            foreach (var sale in sales)
            {
                var exist = dbContext.SaleTransactions.Where(a => a.VehicleNum == sale.PlateNo && a.DateTimeIn == sale.DateTimeIn).AsNoTracking().Count();
                if (exist > 0) continue;

                var saleTrans = new SaleTransaction();
                saleTrans.ReceiptNum = sale.ReceiptNo;
                saleTrans.DateTimeIn = sale.DateTimeIn.Value;
                saleTrans.DateTimeOut = sale.DateTimeOut;
                saleTrans.VehicleNum = sale.PlateNo;
                var customer = customers.FirstOrDefault(a => a.CustomerIdOld == sale.CustomerId);
                if (customer != null)
                {
                    saleTrans.CustomerId = customer.CustomerId;
                    saleTrans.CustomerName = customer.CustomerName;
                }
                var hauler = haulers.FirstOrDefault(a => a.HaulerIdOld == sale.HaulerID);
                if (hauler != null)
                {
                    saleTrans.HaulerId = hauler.HaulerId;
                    saleTrans.HaulerName = hauler.HaulerName;
                }
                var product = products.FirstOrDefault(a => a.ProductIdOld == sale.ProductId);
                if (product != null)
                {
                    saleTrans.ProductId = product.ProductId;
                    saleTrans.ProductDesc = product.ProductDesc;
                    saleTrans.CategoryId = product.Category.CategoryId;
                }
                var baleType = baleTypes.FirstOrDefault(a => a.BaleTypeIdOld == sale.BalesId);
                if (baleType != null)
                {
                    saleTrans.BaleTypeId = baleType.BaleTypeId;
                    saleTrans.BaleTypeDesc = baleType.BaleTypeDesc;
                }
                saleTrans.BaleCount = (int)sale.NoOfBales;
                saleTrans.GrossWt = sale.Gross ?? 0;
                saleTrans.TareWt = sale.Tare ?? 0;
                saleTrans.NetWt = sale.net ?? 0;
                saleTrans.Corrected10 = sale.Corrected_10 ?? 0;
                saleTrans.Corrected12 = sale.Corrected_12 ?? 0;
                saleTrans.Corrected14 = sale.Corrected_14 ?? 0;
                saleTrans.Corrected15 = sale.Corrected_15 ?? 0;
                saleTrans.MC = sale.Moisture ?? 0;
                saleTrans.MCStatus = 1;
                saleTrans.PM = sale.PM ?? 0;
                saleTrans.OT = sale.OutThrow ?? 0;
                saleTrans.Remarks = sale.Remarks;
                saleTrans.SealNum = sale.SealNo;
                saleTrans.DriverName = sale.DriverName;
                var weigher = weighers.FirstOrDefault(a => a.UserAccountIdOld == sale.WeigherIn.Trim());
                if (weigher != null)
                {
                    saleTrans.WeigherInId = weigher.UserAccountId;
                    saleTrans.WeigherInName = weigher.FullName;
                }
                weigher = weighers.FirstOrDefault(a => a.UserAccountIdOld == sale.WeigherOut.Trim());
                if (weigher != null)
                {
                    saleTrans.WeigherOutId = weigher.UserAccountId;
                    saleTrans.WeigherOutName = weigher.FullName;
                }
                saleTrans.IsOfflineIn = sale.Processed_In == "OFFLINE";
                saleTrans.IsOfflineOut = sale.Processed_Out == "OFFLINE";
                saleTrans.PrintCount = (int)(sale.NoOfPrinting ?? 0);
                saleTrans.WeekDay = Convert.ToInt32(sale.Weekday);
                saleTrans.WeekNum = Convert.ToInt32(sale.WeekNo);
                saleTrans.FirstDay = sale.FirstDay ?? DateTime.Now;
                saleTrans.LastDay = sale.LastDay ?? DateTime.Now;
                saleTrans.Trip = sale.TypeOfTrip;
                saleTrans.MoistureSettingsId = 1;
                saleTrans.SignatoryId = 1;

                saleTrans.Returned = sale.Returned ?? false;

                saleTrans.TimeZoneIn = sale.tz_in ?? sale.DateTimeIn.GetTimeZone();
                saleTrans.TimeZoneOut = sale.tz_out ?? sale.DateTimeOut.GetTimeZone();

                dbContext.SaleTransactions.Add(saleTrans);
                dbContext.SaveChanges();

                if (saleTrans.Returned)
                {
                    var returnedVehicle = new ReturnedVehicle();
                    returnedVehicle.BaleCount = Convert.ToDecimal(sale.Plant_BaleCount ?? 0);
                    returnedVehicle.PMAdjustedWt = Convert.ToDecimal(sale.Plant_DedWt ?? 0);
                    returnedVehicle.DiffDay = Convert.ToInt32(sale.Plant_DayInterval ?? 0);
                    returnedVehicle.DiffTime = Convert.ToInt32(sale.Plant_TimeInterval ?? 0);
                    returnedVehicle.DiffCorrected10 = Convert.ToInt32(sale.Plant_DiffMC10 ?? 0);
                    returnedVehicle.DiffCorrected12 = Convert.ToInt32(sale.Plant_DiffMC12 ?? 0);
                    returnedVehicle.DiffNet = Convert.ToInt32(sale.Plant_DiffNet ?? 0);
                    returnedVehicle.DTArrival = sale.DTArrival ?? DateTime.Now;
                    returnedVehicle.DTGuardIn = sale.Plant_Guard_in ?? DateTime.Now;
                    returnedVehicle.DTGuardOut = sale.Plant_Guard_out ?? DateTime.Now;
                    returnedVehicle.MC = sale.Plant_Moisture ?? 0;
                    returnedVehicle.Corrected10 = sale.Plant_MC10 ?? 0;
                    returnedVehicle.Corrected12 = sale.Plant_MC12 ?? 0;
                    returnedVehicle.OT = sale.OutThrow ?? 0;
                    returnedVehicle.PM = sale.PM ?? 0;
                    returnedVehicle.PlantNetWt = sale.Plant_NetWt ?? 0;
                    returnedVehicle.PMAdjustedWt = returnedVehicleRepository.GetPMAdjustedWt(returnedVehicle);
                    returnedVehicle.OTAdjustedWt = returnedVehicleRepository.GetOTAdjustedWt(returnedVehicle);
                    returnedVehicle.Remarks = sale.Plant_Remarks;
                    returnedVehicle.VehicleOrigin = sale.Plant_TruckOrigin;
                    returnedVehicle.TimeVarianceRemarks = sale.time_variance_remarks;
                    var plantUser = weighers.FirstOrDefault(a => a.UserAccountIdOld == sale.Plant_User);
                    if (plantUser != null)
                    {
                        returnedVehicle.UserAccountId = plantUser.UserAccountId;
                          returnedVehicle.UserAccountFullName = plantUser.FullName;
                        }
                    returnedVehicle.SaleId = saleTrans.SaleId;
                    dbContext.ReturnedVehicles.Add(returnedVehicle);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
