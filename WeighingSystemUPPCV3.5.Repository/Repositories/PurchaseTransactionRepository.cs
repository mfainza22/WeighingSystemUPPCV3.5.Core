
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
        private readonly IUserAccountRepository userAccountRepository;
        private readonly IPrintLogRepository printLogRepository;
        private readonly IVehicleRepository vehicleRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IRawMaterialRepository rawMaterialRepository;
        private readonly ISourceRepository sourceRepository;
        private readonly IMoistureReaderRepository moistureReaderRepository;
        private readonly IMoistureSettingsRepository mcRepository;
        private readonly IBaleTypeRepository baleTypeRepository;

        public PurchaseTransactionRepository(DatabaseContext dbContext,
            IUserAccountRepository userAccountRepository,
            IPrintLogRepository printLogRepository,
            IVehicleRepository vehicleRepository,
            ISupplierRepository supplierRepository,
            IRawMaterialRepository rawMaterialRepository,
            ISourceRepository sourceRepository,
            IMoistureReaderRepository moistureReaderRepository,
            IMoistureSettingsRepository mcRepository,
            IBaleTypeRepository baleTypeRepository)
        {
            this.dbContext = dbContext;
            this.userAccountRepository = userAccountRepository;
            this.printLogRepository = printLogRepository;
            this.vehicleRepository = vehicleRepository;
            this.supplierRepository = supplierRepository;
            this.rawMaterialRepository = rawMaterialRepository;
            this.sourceRepository = sourceRepository;
            this.moistureReaderRepository = moistureReaderRepository;
            this.mcRepository = mcRepository;
            this.baleTypeRepository = baleTypeRepository;
        }

        public IQueryable<PurchaseTransaction> Get(PurchaseTransaction parameters = null) => throw new NotImplementedException();

        public PurchaseTransaction GetById(long id)
        {
            return dbContext.PurchaseTransactions.Find(id);
        }

        public PurchaseTransaction GetByReceiptNum(string receiptNum)
        {
            return dbContext.PurchaseTransactions.Where(a => a.ReceiptNum == receiptNum).FirstOrDefault();
        }


        public PurchaseTransaction Update(PurchaseTransaction modifiedPurchase)
        {
            var entity = dbContext.PurchaseTransactions.AsNoTracking().FirstOrDefault(a => a.PurchaseId == modifiedPurchase.PurchaseId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            updateRelatedTableColumns(ref modifiedPurchase);

            var correctedMC = mcRepository.GetCorrectedMC(modifiedPurchase.MC, entity.NetWt);

            entity.BaleCount = modifiedPurchase.BaleCount;
            entity.BaleTypeDesc = modifiedPurchase.BaleTypeDesc;
            entity.BaleTypeId = modifiedPurchase.BaleTypeId;
            entity.CategoryDesc = modifiedPurchase.CategoryDesc;
            entity.CategoryId = modifiedPurchase.CategoryId;
            entity.Corrected10 = correctedMC.Corrected10;
            entity.Corrected12 = correctedMC.Corrected12;
            entity.Corrected14 = correctedMC.Corrected14;
            entity.Corrected15 = correctedMC.Corrected15;
            entity.DriverName = modifiedPurchase.DriverName;
            entity.MC = modifiedPurchase.MC;
            entity.MCStatus = modifiedPurchase.MCStatus;
            entity.MoistureReaderId = modifiedPurchase.MoistureReaderId;
            entity.MoistureReaderDesc = modifiedPurchase.MoistureReaderDesc;
            entity.OT = modifiedPurchase.OT;
            entity.PM = modifiedPurchase.PM;
            entity.RawMaterialId = modifiedPurchase.RawMaterialId;
            entity.RawMaterialDesc = modifiedPurchase.RawMaterialDesc;
            entity.Remarks = modifiedPurchase.Remarks;
            entity.SourceId = modifiedPurchase.SourceId;
            entity.SourceName = modifiedPurchase.SourceName;
            entity.SourceCategoryId = modifiedPurchase.SourceCategoryId;
            entity.SourceCategoryDesc = modifiedPurchase.SourceCategoryDesc;
            entity.SubSupplierName = modifiedPurchase.SubSupplierName;
            entity.SupplierId = modifiedPurchase.SupplierId;
            entity.SupplierName = modifiedPurchase.SupplierName;
            entity.Trip = modifiedPurchase.Trip;
            entity.VehicleNum = modifiedPurchase.VehicleNum;
            entity.VehicleTypeId = modifiedPurchase.VehicleTypeId;
            entity.VehicleTypeCode = modifiedPurchase.VehicleTypeCode;
            entity.WeigherOutId = modifiedPurchase.WeigherOutId;
            entity.WeigherOutName = modifiedPurchase.WeigherOutName;

            entity.MoistureReaderLogsModified = modifiedPurchase.MoistureReaderLogsModified;

            dbContext.PurchaseTransactions.Update(entity);

            if (modifiedPurchase.MoistureReaderLogsModified)
            {
                dbContext.RemoveRange(dbContext.moistureReaderLogs.Where(a => a.TransactionId == modifiedPurchase.PurchaseId));
                //foreach (var moistureReaderLog in modifiedPurchase.MoistureReaderLogs)
                //{
                //    moistureReaderLog.TransactionId = modifiedPurchase.PurchaseId;
                //};

                //dbContext.AddRange(modifiedPurchase.MoistureReaderLogs);
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

            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM PurchaseTransactions");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();

            if (parameters == null) return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };

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

                query = null;
            }

            return serverDataSet;
        }

        public IQueryable<PrintLog> GetPrintLogs(long transactionId)
        {
            return printLogRepository.Get(new PrintLog() { TransactionId = transactionId, TransactionTypeCode = "I" });
        }

        public PurchaseTransaction GetByIdWithMCReaderLogs(long id)
        {
            return dbContext.PurchaseTransactions.Include(a => a.MoistureReaderLogs).AsNoTracking().FirstOrDefault(a => a.PurchaseId == id);
        }

        public void MigrateOldDb(DateTime dtFrom, DateTime dtTo)
        {
            dtFrom = dtFrom.Date;
            dtTo = dtTo.Date + new TimeSpan(23, 59, 59);
            var purchases = dbContext.Purchases.Where(a => a.DateTimeIn >= dtFrom && a.DateTimeIn <= dtTo).OrderBy(a => a.ReceiptNo)
                .GroupJoin(dbContext.RawMaterials.Include(a => a.Category).Where(a => a.RawMaterialIdOld != null),
                    purchase => purchase.MaterialId,
                    material => material.RawMaterialIdOld,
                    (purchase, material) => new { purchase, material })
                .SelectMany(a => a.material.DefaultIfEmpty(),
                (purchase, material) => new { purchase.purchase, material })
                .GroupJoin(dbContext.Suppliers,
                    t => t.purchase.ClientId,
                    supplier => supplier.SupplierIdOld,
                    (purchase, supplier) => new { purchase.purchase, purchase.material, supplier })
                 .SelectMany(a => a.supplier.DefaultIfEmpty(),
                (purchase, supplier) => new { purchase.purchase, purchase.material, supplier })
                .GroupJoin(dbContext.Sources.Include(a => a.SourceCategory),
                    t => t.purchase.SubCat,
                    source => source.SourceDesc,
                    (purchase, source) => new { purchase.purchase, purchase.material, purchase.supplier, source })
                 .SelectMany(a => a.source.DefaultIfEmpty(),
                (purchase, source) => new { purchase.purchase, purchase.material, purchase.supplier, source })
                 .GroupJoin(dbContext.UserAccounts,
                    t => t.purchase.WeigherIn,
                    user => user.UserAccountIdOld,
                    (purchase, user) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, user })
                 .SelectMany(a => a.user.DefaultIfEmpty(),
                (purchase, user) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, userIn = user })
                 .GroupJoin(dbContext.UserAccounts,
                    t => t.purchase.WeigherOut,
                    user => user.UserAccountIdOld,
                    (purchase, user) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, purchase.userIn, user })
                 .SelectMany(a => a.user.DefaultIfEmpty(),
                (purchase, user) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, purchase.userIn, userOut = user })
                  .GroupJoin(dbContext.BaleTypes,
                    t => t.purchase.BalesId,
                    baleType => baleType.BaleTypeIdOld,
                    (purchase, baleType) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, purchase.userIn, purchase.userOut, baleType })
                 .SelectMany(a => a.baleType.DefaultIfEmpty(),
                (purchase, baleType) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, purchase.userIn, purchase.userOut, baleType })
                 .GroupJoin(dbContext.Vehicles.Include(a => a.VehicleType),
                    t => t.purchase.PlateNo,
                    vehicle => vehicle.VehicleNum,
                    (purchase, vehicle) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, purchase.userIn, purchase.userOut, purchase.baleType, vehicle })
                 .SelectMany(a => a.vehicle.DefaultIfEmpty(),
                (purchase, vehicle) => new { purchase.purchase, purchase.material, purchase.supplier, purchase.source, purchase.userIn, purchase.userOut, purchase.baleType, vehicle })
                 .Where(a => dbContext.PurchaseTransactions.Select(a => a.ReceiptNum).Contains(a.purchase.ReceiptNo) == false)
                 .AsNoTracking().ToList();


            var allPurchase = purchases.Select(a => new PurchaseTransaction
            {
                BaleCount = Convert.ToInt32(a.purchase.NoOfBales ?? 0),
                BaleTypeId = a.baleType == null ? 0 : a.baleType.BaleTypeId,
                BaleTypeDesc = a.baleType == null ? null : a.baleType.BaleTypeDesc,
                BalingStationCode = null,
                BalingStationName = null,
                CategoryId = a.material == null ? 0 : a.material.CategoryId,
                CategoryDesc = a.material == null ? null : a.material.Category == null ? null : a.material.Category.CategoryDesc,
                Corrected10 = a.purchase.Corrected_10 ?? 0,
                Corrected12 = a.purchase.Corrected_12 ?? 0,
                Corrected14 = a.purchase.Corrected_14 ?? 0,
                Corrected15 = a.purchase.Corrected_15 ?? 0,
                DateTimeIn = a.purchase.DateTimeIn.Value,
                DateTimeOut = a.purchase.DateTimeOut,
                DriverName = a.purchase.DriverName,
                DRNum = a.purchase.DrNo,
                FactoryWt = a.purchase.Factory_Wt ?? 0,
                FirstDay = a.purchase.FirstDay,
                GrossWt = a.purchase.Gross ?? 0,
                IsOfflineIn = a.purchase.Processed_In == "OFFLINE",
                IsOfflineOut = a.purchase.Processed_In == "OFFLINE",
                LastDay = a.purchase.LastDay,
                MoistureReaderId = null,
                MC = a.purchase.Moisture ?? 0,
                MCStatus = a.purchase.MoistureStatus == 0 ? 10 :
                        a.purchase.MoistureStatus == 1 ? 12 :
                        a.purchase.MoistureStatus == 2 ? 14 :
                        15,
                MoistureReaderDesc = null,
                MoistureSettingsId = 1,
                NetWt = a.purchase.net ?? 0,
                OT = a.purchase.OutThrow ?? 0,
                PM = a.purchase.PM ?? 0,
                PONum = a.purchase.PoNo,
                PrintCount = Convert.ToInt32(a.purchase.NoOfPrinting ?? 0),
                Price = a.purchase.Price ?? 0,
                RawMaterialId = a.material == null ? 0 : a.material.RawMaterialId,
                RawMaterialDesc = a.material == null ? "" : a.material.RawMaterialDesc,
                ReceiptNum = a.purchase.ReceiptNo,
                Remarks = a.purchase.Remarks,
                SignatoryId = 1,
                SourceCategoryDesc = a.source == null ? null : a.source.SourceCategory == null ? null : a.source.SourceCategory.Description,
                SourceCategoryId = a.source == null ? 0 : a.source.SourceCategory == null ? 0 : a.source.SourceCategory.SourceCategoryId,
                SourceId = a.source == null ? 0 : a.source.SourceId,
                SourceName = a.source == null ? null : a.source.SourceDesc,
                SubSupplierName = a.purchase.SubSupplier,
                SupplierId = a.supplier == null ? 0 : a.supplier.SupplierId,
                SupplierName = a.supplier == null ? null : a.supplier.SupplierName,
                TimeZoneIn = a.purchase.tz_in == null ? "GMT+08:00 PH" : a.purchase.tz_in,
                TimeZoneOut = a.purchase.tz_out == null ? "GMT+08:00 PH" : a.purchase.tz_out,
                TareWt = a.purchase.Tare ?? 0,
                Trip = a.purchase.TypeOfTrip,
                VehicleNum = a.purchase.PlateNo,
                VehicleTypeId = a.vehicle == null ? null : (Nullable<long>)a.vehicle.VehicleTypeId,
                VehicleTypeCode = a.vehicle == null ? null : a.vehicle.VehicleType == null ? null : a.vehicle.VehicleType.VehicleTypeCode,
                WeekDay = Convert.ToInt32(a.purchase.Weekday ?? 0),
                WeekNum = Convert.ToInt32(a.purchase.WeekNo),
                WeigherInId = a.userIn == null ? null : a.userIn.UserAccountId,
                WeigherInName = a.userIn == null ? null : a.userIn.FullName,
                WeigherOutId = a.userOut == null ? null : a.userOut.UserAccountId,
                WeigherOutName = a.userOut == null ? null : a.userOut.FullName,
                MCDate = a.purchase.MC1DT,
                MoistureReaderLogs = new List<MoistureReaderLog>() {
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC1DT,LogNum = 1,MC = a.purchase.MC1??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC2DT,LogNum = 2,MC = a.purchase.MC2??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC3DT,LogNum = 3,MC = a.purchase.MC3??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC4DT,LogNum = 4,MC = a.purchase.MC4??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC5DT,LogNum = 5,MC = a.purchase.MC5??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC6DT,LogNum = 6,MC = a.purchase.MC6??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC7DT,LogNum = 7,MC = a.purchase.MC7??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC8DT,LogNum = 8,MC = a.purchase.MC8??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC9DT,LogNum = 9,MC = a.purchase.MC9??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC10DT,LogNum = 10,MC = a.purchase.MC10??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC11DT,LogNum = 11,MC = a.purchase.MC11??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC12DT,LogNum = 12,MC = a.purchase.MC12??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC13DT,LogNum = 13,MC = a.purchase.MC13??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC14DT,LogNum = 14,MC = a.purchase.MC14??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC15DT,LogNum = 15,MC = a.purchase.MC15??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC16DT,LogNum = 16,MC = a.purchase.MC16??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC17DT,LogNum = 17,MC = a.purchase.MC17??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC18DT,LogNum = 18,MC = a.purchase.MC18??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC19DT,LogNum = 19,MC = a.purchase.MC19??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC20DT,LogNum = 20,MC = a.purchase.MC20??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC21DT,LogNum = 21,MC = a.purchase.MC21??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC22DT,LogNum = 22,MC = a.purchase.MC22??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC23DT,LogNum = 23,MC = a.purchase.MC23??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC24DT,LogNum = 24,MC = a.purchase.MC24??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC25DT,LogNum = 25,MC = a.purchase.MC25??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC26DT,LogNum = 26,MC = a.purchase.MC26??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC27DT,LogNum = 27,MC = a.purchase.MC27??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC28DT,LogNum = 28,MC = a.purchase.MC28??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC29DT,LogNum = 29,MC = a.purchase.MC29??0 },
                    new MoistureReaderLog() { TransactionId = 0, DTLog = a.purchase.MC30DT,LogNum = 30,MC = a.purchase.MC30??0 },
                 }
            }).OrderBy(a => a.DateTimeIn).ToList();


            for (var i = 0; i <= allPurchase.Count - 1; i++)
            {
                dbContext.AddRange(allPurchase[i]);
                dbContext.SaveChanges();
                dbContext.Entry(allPurchase[i]).State = EntityState.Detached;
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

        public void updateRelatedTableColumns(ref PurchaseTransaction model)
        {
            var vehicleNum = model.VehicleNum;
            var vehicle = vehicleRepository.Get()
    .Include(a => a.VehicleType).DefaultIfEmpty()
    .Where(a => a.VehicleNum == vehicleNum)
    .Select(a => new { a.VehicleNum, a.VehicleTypeId, VehicleTypeCode = a.VehicleType == null ? "" : a.VehicleType.VehicleTypeCode }).ToList().FirstOrDefault();
            model.VehicleTypeId = vehicle?.VehicleTypeId ?? 0;
            model.VehicleTypeCode = vehicle?.VehicleTypeCode;

            var supplierId = model.SupplierId;
            model.SupplierName = supplierRepository.Get()
             .Where(a => a.SupplierId == supplierId).Select(a => a.SupplierName).FirstOrDefault();

            var rawMaterialId = model.RawMaterialId;
            var material = rawMaterialRepository.Get()
                .Where(a => a.RawMaterialId == rawMaterialId)
                .Include(a => a.Category).DefaultIfEmpty()
                .Select(a => new { a.RawMaterialDesc, a.CategoryId, CategoryDesc = a.Category == null ? null : a.Category.CategoryDesc })
                .FirstOrDefault();
            model.RawMaterialDesc = material?.RawMaterialDesc;
            model.CategoryId = material?.CategoryId ?? 0;
            model.CategoryDesc = material?.CategoryDesc;

            var sourceId = model.SourceId;
            var source = sourceRepository.Get()
                .Where(a => a.SourceId == sourceId)
                .Include(a => a.SourceCategory).DefaultIfEmpty()
                .Select(a => new { a.SourceDesc, a.SourceCategoryId, SourceCategoryDesc = a.SourceCategory == null ? null : a.SourceCategory.Description })
                .FirstOrDefault();
            model.SourceName = source?.SourceDesc;
            model.SourceCategoryId = source?.SourceCategoryId ?? 0;
            model.SourceCategoryDesc = source?.SourceCategoryDesc;

            var msId = model.MoistureReaderId;
            model.MoistureReaderDesc = moistureReaderRepository.Get()
                .Where(a => a.MoistureReaderId == msId).Select(a => a.Description).FirstOrDefault();


            var userAccountId = model.WeigherOutId;
            model.WeigherOutName = userAccountRepository.Get().Where(a => a.UserAccountId == userAccountId)
                .Select(a => a.FullName).FirstOrDefault();

        }

    }
}
