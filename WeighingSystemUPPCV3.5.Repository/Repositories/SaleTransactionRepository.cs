
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
                if (parameters.Returned.Value)
                {
                    whereClauses.Add($"(SELECT COUNT(*) FROM ReturnedVehicles Where ReturnedVehicles.SaleId = SaleTransactions.SaleId) > 0");
                } else
                {
                    whereClauses.Add($"(SELECT COUNT(*) FROM ReturnedVehicles Where ReturnedVehicles.SaleId = SaleTransactions.SaleId) = 0");
                }
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
            #region Old Sale Migration
            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var sales = dbContext.Sales.Where(a => a.DateTimeIn >= dtFrom && a.DateTimeIn <= dtTo).OrderBy(a => a.ReceiptNo)
                .GroupJoin(dbContext.Products.Include(a => a.Category).Where(a => a.ProductIdOld != null),
                    sale => sale.ProductId,
                    product => product.ProductIdOld,
                    (sale, product) => new { sale, product })
                .SelectMany(a => a.product.DefaultIfEmpty(),
                (sale, product) => new { sale.sale, product })
                .GroupJoin(dbContext.Customers,
                    t => t.sale.CustomerId,
                    customer => customer.CustomerIdOld,
                    (sale, customer) => new { sale.sale, sale.product, customer })
                 .SelectMany(a => a.customer.DefaultIfEmpty(),
                (sale, customer) => new { sale.sale, sale.product, customer })
                  .GroupJoin(dbContext.Haulers,
                    t => t.sale.HaulerID,
                    hauler => hauler.HaulerIdOld,
                    (sale, hauler) => new { sale.sale, sale.product, sale.customer, hauler })
                 .SelectMany(a => a.hauler.DefaultIfEmpty(),
                (sale, hauler) => new { sale.sale, sale.product, sale.customer, hauler })
                 .GroupJoin(dbContext.UserAccounts,
                    t => t.sale.WeigherIn,
                    user => user.UserAccountIdOld,
                    (sale, user) => new { sale.sale, sale.product, sale.customer, sale.hauler, user })
                 .SelectMany(a => a.user.DefaultIfEmpty(),
                (sale, user) => new { sale.sale, sale.product, sale.customer, sale.hauler, userIn = user })
                 .GroupJoin(dbContext.UserAccounts,
                    t => t.sale.WeigherOut,
                    user => user.UserAccountIdOld,
                    (sale, user) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, user })
                 .SelectMany(a => a.user.DefaultIfEmpty(),
                (sale, user) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, userOut = user })
                  .GroupJoin(dbContext.BaleTypes,
                    t => t.sale.BalesId,
                    baleType => baleType.BaleTypeIdOld,
                    (sale, baleType) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, baleType })
                 .SelectMany(a => a.baleType.DefaultIfEmpty(),
                (sale, baleType) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, baleType })
                 .GroupJoin(dbContext.Vehicles.Include(a => a.VehicleType),
                    t => t.sale.PlateNo,
                    vehicle => vehicle.VehicleNum,
                    (sale, vehicle) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, sale.baleType, vehicle })
                 .SelectMany(a => a.vehicle.DefaultIfEmpty(),
                (sale, vehicle) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, sale.baleType, vehicle })
                 .GroupJoin(dbContext.UserAccounts.Select(a=>new { a.UserAccountId,a.UserAccountIdOld, a.FullName }).DefaultIfEmpty(),
                    t => t.sale.Plant_User,
                    plantUser => plantUser.UserAccountIdOld,
                    (sale, plantUser) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, sale.baleType, sale.vehicle,plantUser })
                 .SelectMany(a => a.plantUser.DefaultIfEmpty(),
                (sale, plantUser) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, sale.baleType, sale.vehicle,plantUser })
                 .AsNoTracking().ToList();


            var allSale = sales.Select(a => new SaleTransaction
            {
                SaleId = 0,
                BaleCount = Convert.ToInt32(a.sale.NoOfBales ?? 0),
                BaleTypeId = a.baleType == null ? 0 : a.baleType.BaleTypeId,
                BaleTypeDesc = a.baleType == null ? null : a.baleType.BaleTypeDesc,
                BalingStationCode = null,
                BalingStationName = null,
                CategoryId = a.product == null ? 0 : a.product.CategoryId,
                CategoryDesc = a.product == null ? null : a.product.Category == null ? null : a.product.Category.CategoryDesc,
                Corrected10 = a.sale.Corrected_10 ?? 0,
                Corrected12 = a.sale.Corrected_12 ?? 0,
                Corrected14 = a.sale.Corrected_14 ?? 0,
                Corrected15 = a.sale.Corrected_15 ?? 0,
                DateTimeIn = a.sale.DateTimeIn.Value,
                DateTimeOut = a.sale.DateTimeOut,
                DriverName = a.sale.DriverName,
                FirstDay = a.sale.FirstDay.Value,
                GrossWt = a.sale.Gross ?? 0,
                TareWt = a.sale.Tare ?? 0,
                NetWt = a.sale.net ?? 0,
                IsOfflineIn = a.sale.Processed_In == "OFFLINE",
                IsOfflineOut = a.sale.Processed_In == "OFFLINE",
                LastDay = a.sale.LastDay.Value,
                MoistureReaderId = null,
                MC = a.sale.Moisture ?? 0,
                MCStatus = Convert.ToInt32(a.sale.MoistureStatus ?? 0),
                MoistureReaderDesc = null,
                MoistureSettingsId = 1,
                OT = a.sale.OutThrow ?? 0,
                PM = a.sale.PM ?? 0,
                Price = 0,
                PrintCount = Convert.ToInt32(a.sale.NoOfPrinting ?? 0),
                ProductId = a.product == null ? 0 : a.product.ProductId,
                ProductDesc = a.product == null ? "" : a.product.ProductDesc,
                ReceiptNum = a.sale.ReceiptNo,
                Remarks = a.sale.Remarks,
                SealNum = a.sale.SealNo,
                SignatoryId = 1,
                CustomerId = a.customer == null ? 0 : a.customer.CustomerId,
                CustomerName = a.customer == null ? null : a.customer.CustomerName,
                HaulerId = a.hauler == null ? 0 : a.hauler.HaulerId,
                HaulerName = a.hauler == null ? null : a.hauler.HaulerName,
                TimeZoneIn = a.sale.tz_in == null ? "GMT+08:00 PH" : a.sale.tz_in,
                TimeZoneOut = a.sale.tz_out == null ? "GMT+08:00 PH" : a.sale.tz_out,
                Trip = a.sale.TypeOfTrip,
                VehicleNum = a.sale.PlateNo,
                VehicleTypeId = a.vehicle == null ? null : (Nullable<long>)a.vehicle.VehicleTypeId,
                VehicleTypeCode = a.vehicle == null ? null : a.vehicle.VehicleType == null ? null : a.vehicle.VehicleType.VehicleTypeCode,
                WeekDay = Convert.ToInt32(a.sale.Weekday ?? "0"),
                WeekNum = Convert.ToInt32(a.sale.WeekNo??0),
                WeigherInId = a.userIn == null ? null : a.userIn.UserAccountId,
                WeigherInName = a.userIn == null ? null : a.userIn.FullName,
                WeigherOutId = a.userOut == null ? null : a.userOut.UserAccountId,
                WeigherOutName = a.userOut == null ? null : a.userOut.FullName,
                ReturnedVehicle = a.sale.Returned ?? false == true ? null : new ReturnedVehicle()
                {
                    BaleCount = Convert.ToInt32(a.sale.Plant_BaleCount ?? 0),
                    Corrected10 = a.sale.Plant_MC10 ?? 0,
                    Corrected12 = a.sale.Plant_MC12 ?? 0,
                    Corrected14 = 0,
                    DiffCorrected10 = a.sale.Plant_DiffMC10 ?? 0,
                    DiffCorrected12 = a.sale.Plant_DiffMC12 ?? 0,
                    DiffDay = Convert.ToInt32(a.sale.Plant_DayInterval ?? 0),
                    DiffTime = a.sale.Plant_TimeInterval ?? 0,
                    DiffNet = a.sale.Plant_DiffNet ?? 0,
                    DTArrival = a.sale.DTArrival?? DateTime.Now,
                    DTGuardIn = a.sale.Plant_Guard_in,
                    DTGuardOut = a.sale.Plant_Guard_out,
                    DTOutToPlant = null,
                    MC = a.sale.Plant_Moisture ?? 0,
                    PM = a.sale.PM ?? 0,
                    PlantNetWt = a.sale.Plant_NetWt ?? 0,
                    PMAdjustedWt = (a.sale.PM ?? 0) > 5 ? (Math.Round(((a.sale.PM ?? 0) - 0.5M) * a.sale.Plant_NetWt ?? 0) / 100) : 0,
                    OT = a.sale.OutThrow ?? 0,
                    OTAdjustedWt = (a.sale.OutThrow ?? 0) > 5 ? (Math.Round(((a.sale.OutThrow ?? 0) - 0.5M) * a.sale.Plant_NetWt ?? 0) / 100) : 0,
                    Remarks = a.sale.Plant_Remarks??String.Empty,
                    TimeVarianceRemarks = a.sale.time_variance_remarks??String.Empty,
                    UserAccountId = a.plantUser != null ? a.plantUser.UserAccountId : null,
                    UserAccountFullName = null,
                    VehicleOrigin = a.sale.Plant_TruckOrigin??String.Empty,
                }
            }).OrderBy(a => a.DateTimeIn).ToList();

            for (var i = 0; i <= allSale.Count - 1; i++)
            {
                dbContext.AddRange(allSale[i]);
                dbContext.SaveChanges();
                dbContext.Entry(allSale[i]).State = EntityState.Detached;
            }
        }
    }
}
