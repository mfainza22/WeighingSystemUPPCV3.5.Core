
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
        private readonly IVehicleRepository vehicleRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IHaulerRepository haulerRepository;
        private readonly IProductRepository productRepository;
        private readonly IMoistureReaderRepository moistureReaderRepository;
        private readonly IMoistureSettingsRepository mcRepository;
        private readonly ILogger<SaleTransactionRepository> logger;
        private readonly IBalingStationRepository balingStationRepository;
        private readonly IUserAccountRepository userAccountRepository;

        public SaleTransactionRepository(DatabaseContext dbContext,
            ILogger<SaleTransactionRepository> logger,
            IBalingStationRepository balingStationRepository,
            IUserAccountRepository  userAccountRepository,
            IPrintLogRepository printLogRepository,
            IBaleRepository baleRepository,
            IVehicleRepository vehicleRepository,
            ICustomerRepository customerRepository,
            IHaulerRepository haulerRepository,
            IProductRepository productRepository,
            IMoistureReaderRepository moistureReaderRepository,
            IMoistureSettingsRepository moistureSettingsRepository
            )
        {
            this.dbContext = dbContext;
            this.printLogRepository = printLogRepository;
            this.baleRepository = baleRepository;
            this.vehicleRepository = vehicleRepository;
            this.customerRepository = customerRepository;
            this.haulerRepository = haulerRepository;
            this.productRepository = productRepository;
            this.moistureReaderRepository = moistureReaderRepository;
            this.mcRepository = moistureSettingsRepository;
            this.logger = logger;
            this.balingStationRepository = balingStationRepository;
            this.userAccountRepository = userAccountRepository;
        }

        public IQueryable<SaleTransaction> Get(SaleTransaction parameters = null)
        {
            return dbContext.SaleTransactions;
        }

        public SaleTransaction GetById(long id) => throw new NotImplementedException();

        public SaleTransaction GetById(long id,bool includeBales = false)
        {
            var model = dbContext.SaleTransactions.AsNoTracking().Where(a => a.SaleId == id);
            if (includeBales) model = model.Include(a => a.SaleBales).ThenInclude(a => a.Bale);
            model = model.Include(a => a.ReturnedVehicle);
            var result = model.FirstOrDefault();
            if (result != null)
            {
                result.SaleBales = result?.SaleBales == null ? new List<SaleBale>() : result.SaleBales;
                result.SaleBales = result.SaleBales.Where(a => a.Bale != null).ToList();
            }
   
            return result;
        }

        public SaleTransaction GetByReceiptNum(string receiptNum)
        {
            return dbContext.SaleTransactions.AsNoTracking().Where(a => a.ReceiptNum == receiptNum).FirstOrDefault();
        }

        public IQueryable<PrintLog> GetPrintLogs(long transactionId)
        {
            return printLogRepository.Get(new PrintLog() { TransactionId = transactionId, TransactionTypeCode = "O" });
        }

        public SaleTransaction Update(SaleTransaction modifiedSale)
        {
            var entity = dbContext.SaleTransactions.AsNoTracking().FirstOrDefault(a => a.SaleId == modifiedSale.SaleId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            updateRelatedTableColumns(ref modifiedSale);

            var correctedMC = mcRepository.GetCorrectedMC(modifiedSale.MC, entity.NetWt);

            entity.BaleCount = modifiedSale.BaleCount;
            entity.BaleTypeDesc = modifiedSale.BaleTypeDesc;
            entity.BaleTypeId = modifiedSale.BaleTypeId;
            entity.CategoryDesc = modifiedSale.CategoryDesc;
            entity.CategoryId = modifiedSale.CategoryId;
            entity.Corrected10 = correctedMC.Corrected10;
            entity.Corrected12 = correctedMC.Corrected12;
            entity.Corrected14 = correctedMC.Corrected14;
            entity.Corrected15 = correctedMC.Corrected15;
            entity.CustomerId = modifiedSale.CustomerId;
            entity.CustomerName = modifiedSale.CustomerName;
            entity.DriverName = modifiedSale.DriverName;
            entity.HaulerId = modifiedSale.HaulerId;
            entity.HaulerName = modifiedSale.HaulerName;
            entity.MC = modifiedSale.MC;
            //entity.MCStatus = modifiedSale.MCStatus;
            entity.MoistureReaderId = modifiedSale.MoistureReaderId;
            entity.MoistureReaderDesc = modifiedSale.MoistureReaderDesc;
            entity.OT = modifiedSale.OT;
            entity.PM = modifiedSale.PM;
            entity.ProductId = modifiedSale.ProductId;
            entity.ProductDesc = modifiedSale.ProductDesc;
            entity.Remarks = modifiedSale.Remarks;
            entity.SealNum = modifiedSale.SealNum;
            entity.Trip = modifiedSale.Trip;
            entity.VehicleNum = modifiedSale.VehicleNum;
            entity.VehicleTypeId = modifiedSale.VehicleTypeId;
            entity.VehicleTypeCode = modifiedSale.VehicleTypeCode;
            entity.WeigherOutId = modifiedSale.WeigherOutId;
            entity.WeigherOutName = modifiedSale.WeigherOutName;

            dbContext.SaleTransactions.Update(entity);
            dbContext.SaveChanges();
            return entity;
        }

        public bool Delete(SaleTransaction model)
        {
            dbContext.SaleTransactions.Remove(model);
            dbContext.SaveChanges();
            baleRepository.CheckAndCreateBaleOverageReminder();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.SaleTransactions.Where(a => id.Contains(a.SaleId.ToString()));

            dbContext.SaleTransactions.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            baleRepository.CheckAndCreateBaleOverageReminder();
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
                }
                else
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

        public Dictionary<string, string> Validate(SaleTransaction model)
        {
            throw new NotImplementedException();
        }

        public DataSet PrintReceipt(PrintReceiptModel model)
        {
            if (model.IsReprinted) printLogRepository.Create(model);

            var serverDataSet = new DataSet();
            serverDataSet.EnforceConstraints = false;
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.SaleTransactions)));
            serverDataSet.Tables.Add(new DataTable(nameof(dbContext.PurchaseTransactions)));

            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                var query = string.Empty;
                var sa = new SqlDataAdapter();

                query = dbContext.SaleTransactions
              .Where(a => a.SaleId == model.TransactionId).ToQueryString();
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.SaleTransactions));
                sa.Dispose();

                query = $"SELECT {model.TransactionId} AS PurchaseId";
                sa = new SqlDataAdapter(query.ToString(), sqlConn);
                sa.Fill(serverDataSet, nameof(dbContext.PurchaseTransactions));
                sa.Dispose();

                query = null;
            }

            return serverDataSet;
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

        public List<SaleBale> UpdateBales(long id,List<SaleBale> newSaleBales)
        {
            var entity = dbContext.SaleTransactions.Include(a=>a.SaleBales).FirstOrDefault(a => a.SaleId == id);


            var deletedBales = entity.SaleBales.Select(a=>a.BaleId).Except(newSaleBales.Select(a=>a.BaleId))
                .Select(a=> new SaleBale() { BaleId = a,SaleId = entity.SaleId }).ToList();

            for (var i = 0; i <= deletedBales.Count()-1;i++)
                entity.SaleBales.Remove(entity.SaleBales.FirstOrDefault(a => deletedBales.Select(a => a.BaleId).Contains(a.BaleId)));

            var newBales = newSaleBales.Select(a => a.BaleId).Except(entity.SaleBales.Select(a => a.BaleId))
                .Select(a => new SaleBale() { BaleId = a, SaleId = entity.SaleId }).ToList();

            for (var i = 0; i <= newBales.Count() - 1; i++)
                entity.SaleBales.Add(newBales[i]);

            dbContext.SaleTransactions.Update(entity);

            dbContext.SaveChanges();

            baleRepository.CheckAndCreateBaleOverageReminder();

            return dbContext.SaleTransactions.Where(a => a.SaleId == entity.SaleId)
                .Include(a => a.SaleBales).ThenInclude(a => a.Bale)
                .AsNoTracking().FirstOrDefault().SaleBales.ToList();
  
        }

        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);

  
            var sales = dbContext.Sales.Where(a => a.DateTimeIn >= dtFrom && a.DateTimeIn <= dtTo).OrderBy(a => a.ReceiptNo)
                .Include(a => a.BalesInv)
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
                 .GroupJoin(dbContext.UserAccounts.Select(a => new { a.UserAccountId, a.UserAccountIdOld, a.FullName }).DefaultIfEmpty(),
                    t => t.sale.Plant_User,
                    plantUser => plantUser.UserAccountIdOld,
                    (sale, plantUser) => new { sale.sale, sale.product, sale.customer, sale.hauler, sale.userIn, sale.userOut, sale.baleType, sale.vehicle, plantUser })
                 .SelectMany(a => a.plantUser.DefaultIfEmpty(),
                (sale, plantUser) => new
                {
                    sale.sale,
                    sale.product,
                    sale.customer,
                    sale.hauler,
                    sale.userIn,
                    sale.userOut,
                    sale.baleType,
                    sale.vehicle,
                    plantUser
                })
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
                WeekNum = Convert.ToInt32(a.sale.WeekNo ?? 0),
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
                    DTArrival = a.sale.DTArrival ?? DateTime.Now,
                    DTGuardIn = a.sale.Plant_Guard_in,
                    DTGuardOut = a.sale.Plant_Guard_out,
                    DTOutToPlant = null,
                    MC = a.sale.Plant_Moisture ?? 0,
                    PM = a.sale.PM ?? 0,
                    PlantNetWt = a.sale.Plant_NetWt ?? 0,
                    PMAdjustedWt = (a.sale.PM ?? 0) > 5 ? (Math.Round(((a.sale.PM ?? 0) - 0.5M) * a.sale.Plant_NetWt ?? 0) / 100) : 0,
                    OT = a.sale.OutThrow ?? 0,
                    OTAdjustedWt = (a.sale.OutThrow ?? 0) > 5 ? (Math.Round(((a.sale.OutThrow ?? 0) - 0.5M) * a.sale.Plant_NetWt ?? 0) / 100) : 0,
                    Remarks = a.sale.Plant_Remarks ?? String.Empty,
                    TimeVarianceRemarks = a.sale.time_variance_remarks ?? String.Empty,
                    UserAccountId = a.plantUser != null ? a.plantUser.UserAccountId : null,
                    UserAccountFullName = null,
                    VehicleOrigin = a.sale.Plant_TruckOrigin ?? String.Empty,
                },
                SaleBales = a.sale.BalesInv.Select(b => new SaleBale() { BaleId = b.BaleIdNew }).ToList()
            }).OrderBy(a => a.DateTimeIn).ToList();

            for (var i = 0; i <= allSale.Count - 1; i++)
            {
                dbContext.AddRange(allSale[i]);
                dbContext.SaveChanges();
                dbContext.Entry(allSale[i]).State = EntityState.Detached;
            }
        }

        private void updateRelatedTableColumns(ref SaleTransaction model)
        {
            var vehicleNum = model.VehicleNum;
            var vehicle = vehicleRepository.Get()
    .Include(a => a.VehicleType).DefaultIfEmpty()
    .Where(a => a.VehicleNum == vehicleNum)
    .Select(a => new { a.VehicleNum, a.VehicleTypeId, VehicleTypeCode = a.VehicleType == null ? "" : a.VehicleType.VehicleTypeCode }).ToList().FirstOrDefault();
            model.VehicleTypeId = vehicle?.VehicleTypeId ?? 0;
            model.VehicleTypeCode = vehicle?.VehicleTypeCode;

            var customerId = model.CustomerId;
            model.CustomerName = customerRepository.Get()
             .Where(a => a.CustomerId == customerId).Select(a => a.CustomerName).FirstOrDefault();

            var haulerId = model.HaulerId;
            model.HaulerName = haulerRepository.Get()
             .Where(a => a.HaulerId == haulerId).Select(a => a.HaulerName).FirstOrDefault();


            var productId = model.ProductId;
            var product = productRepository.Get()
                .Where(a => a.ProductId == productId)
                .Include(a => a.Category).DefaultIfEmpty()
                .Select(a => new { a.ProductDesc, a.CategoryId, CategoryDesc = a.Category == null ? null : a.Category.CategoryDesc })
                .FirstOrDefault();
            model.ProductDesc = product?.ProductDesc;
            model.CategoryId = product?.CategoryId ?? 0;
            model.CategoryDesc = product?.CategoryDesc;

            var msId = model.MoistureReaderId;
            model.MoistureReaderDesc = moistureReaderRepository.Get()
                .Where(a => a.MoistureReaderId == msId).Select(a => a.Description).FirstOrDefault();

            var userAccountId = model.WeigherOutId;
            model.WeigherOutName = userAccountRepository.Get().Where(a => a.UserAccountId == userAccountId)
                .Select(a => a.FullName).FirstOrDefault();

        }
    }
}
