
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.Interfaces;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SysUtility;
using SysUtility.Extensions;
using SysUtility.Models;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class InyardRepository : IInyardRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IBalingStationRepository balingStationRepository;
        private readonly IUserAccountRepository userAccountRepository;
        private readonly IReferenceNumberRepository refNumRepository;
        private readonly IAuditLogRepository auditLogRepository;
        private readonly IAuditLogEventRepository auditLogEventRepository;
        private readonly ISubSupplierRepository subSupplierRepository;
        private readonly IMoistureSettingsRepository mcRepo;
        private readonly IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository;
        private readonly IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository;
        private readonly IVehicleRepository vehicleRepository;
        private readonly IVehicleTypeRepository vehicleTypeRepository;
        private readonly IRawMaterialRepository rawMaterialRepository;
        private readonly IProductRepository productRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IHaulerRepository haulerRepository;
        private readonly IMoistureReaderRepository moistureReaderRepository;
        private readonly IBaleTypeRepository baleTypeRepository;
        private readonly ISourceRepository sourceRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IBaleRepository baleRepository;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;

        public InyardRepository(DatabaseContext dbContext,
            IBalingStationRepository balingStationRepository,
            IUserAccountRepository userAccountRepository,
            IReferenceNumberRepository refNumRepository,
            IAuditLogRepository auditLogRepository,
            IAuditLogEventRepository auditLogEventRepository,
            ISubSupplierRepository subSupplierRepository,
            IMoistureSettingsRepository mcRepo,
            IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository,
            IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository,
            IVehicleRepository vehicleRepository,
            IVehicleTypeRepository vehicleTypeRepository,
            IRawMaterialRepository rawMaterialRepository,
            IProductRepository productRepository,
            ISupplierRepository supplierRepository,
            IHaulerRepository haulerRepository,
            IMoistureReaderRepository moistureReaderRepository,
            IBaleTypeRepository baleTypeRepository,
            ISourceRepository sourceRepository,
            ICustomerRepository customerRepository,
            IBaleRepository baleRepository,
            IPurchaseOrderRepository purchaseOrderRepository)
        {
            this.dbContext = dbContext;
            this.balingStationRepository = balingStationRepository;
            this.userAccountRepository = userAccountRepository;
            this.refNumRepository = refNumRepository;
            this.auditLogRepository = auditLogRepository;
            this.auditLogEventRepository = auditLogEventRepository;
            this.subSupplierRepository = subSupplierRepository;
            this.mcRepo = mcRepo;
            this.purchaseGrossWtRestrictionRepository = purchaseGrossWtRestrictionRepository;
            this.vehicleDeliveryRestrictionRepository = vehicleDeliveryRestrictionRepository;
            this.vehicleRepository = vehicleRepository;
            this.vehicleTypeRepository = vehicleTypeRepository;
            this.rawMaterialRepository = rawMaterialRepository;
            this.productRepository = productRepository;
            this.supplierRepository = supplierRepository;
            this.haulerRepository = haulerRepository;
            this.moistureReaderRepository = moistureReaderRepository;
            this.baleTypeRepository = baleTypeRepository;
            this.sourceRepository = sourceRepository;
            this.customerRepository = customerRepository;
            this.baleRepository = baleRepository;
            this.purchaseOrderRepository = purchaseOrderRepository;
        }

        public Inyard WeighIn(Inyard model)
        {
            var refNum = refNumRepository.Get().FirstOrDefault();
            model.InyardNum = refNum?.InyardNum;

            updateRelatedTableColumns(ref model);

            var newInyard = new Inyard()
            {
                BaleCount = model.BaleCount,
                BaleTypeDesc = baleTypeRepository.Get().Where(a => a.BaleTypeId == model.BaleTypeId).Take(1).Select(a => a.BaleTypeDesc).FirstOrDefault(),
                BaleTypeId = model.BaleTypeId,
                BalingStationNum = model.BalingStationNum,
                BalingStationCode = model.BalingStationCode,
                BalingStationName = model.BalingStationName,
                CategoryDesc = model.CategoryDesc,
                CategoryId = model.CategoryId,
                ClientId = model.ClientId,
                ClientName = model.ClientName,
                CommodityDesc = model.CommodityDesc,
                CommodityId = model.CommodityId,
                DateTimeIn = model.DateTimeIn,
                DateTimeOut = null,
                DriverName = model.DriverName?.ToUpper(),
                DRNum = model.DRNum?.ToUpper(),
                GrossWt = model.GrossWt,
                HaulerId = model.HaulerId,
                InyardNum = model.InyardNum,

                IsOfflineIn = model.IsOfflineIn,
                IsOfflineOut = model.IsOfflineOut,
                MC = model.MC,
                MCStatus = model.MCStatus,
                MoistureReaderProcess = model.MoistureReaderProcess,
                MoistureReaderDesc = model.MoistureReaderDesc,
                MoistureReaderId = model.MoistureReaderId,
                MoistureSettingsId = 1,
                NetWt = model.NetWt,
                OT = model.OT,
                PlantMC = model.PlantMC,
                PlantNetWt = model.PlantNetWt,
                PlantTruckOrigin = model.PlantTruckOrigin?.ToUpper(),
                PM = model.PM,
                PurchaseOrderId = model.PurchaseOrderId,
                PONum = model.PONum,
                POType = model.POType,
                Price = model.Price,
                Remarks = model.Remarks?.ToUpper(),
                SealNum = model.SealNum?.ToUpper(),
                SignatoryId = 1,
                SourceCategoryDesc = model.SourceCategoryDesc,
                SourceCategoryId = model.SourceCategoryId,
                SourceId = model.SourceId,
                SourceName = model.SourceName,
                SubSupplierName = model.SubSupplierName?.ToUpper(),
                TareWt = model.TareWt,
                TimeZoneIn = model.DateTimeIn.GetTimeZone(),
                TimeZoneOut = null,
                TransactionProcess = model.TransactionProcess,
                TransactionTypeCode = model.TransactionTypeCode,
                Trip = model.Trip?.ToUpper(),
                VehicleNum = model.VehicleNum?.ToUpper(),
                VehicleTypeCode = model.VehicleTypeCode,
                VehicleTypeId = model.VehicleTypeId,
                WeigherInId = model.WeigherInId,
                WeigherInName = model.WeigherInName,
                WeigherOutId = null,
                WeigherOutName = null
            };

            dbContext.Inyards.Add(newInyard);

            refNum.InyardNum = String.Format(StringFormats.REFNO_FORMAT, Convert.ToInt32(refNum.InyardNum) + 1);
            dbContext.ReferenceNumbers.Update(refNum);
            dbContext.SaveChanges();

            if (model.TransactionTypeCode == "I" && model.IsOfflineIn == false)
            {
                var vd = new VehicleDeliveryRestriction()
                {
                    VehicleNum = model.VehicleNum,
                    CommodityId = model.CommodityId,
                    DateTimeIn = model.DateTimeIn
                };

                vehicleDeliveryRestrictionRepository.Create(vd);

                var pg = new PurchaseGrossWtRestriction()
                {
                    VehicleNum = model.VehicleNum,
                    Weight = model.InitialWt,
                    DateTimeIn = model.DateTimeIn
                };

                purchaseGrossWtRestrictionRepository.Create(pg);
            }

            if (model.IsOfflineIn)
            {
                var auditLog = new AuditLog()
                {
                    AuditLogEventId = auditLogEventRepository.GetOfflineInEventId(),
                    UserAccountId = model.WeigherInId,
                    Notes = formatOfflineInEvent(model)
                };
                auditLogRepository.Create(auditLog);
            }
            return newInyard;
        }

        public PurchaseTransaction WeighoutPurchase(Inyard model)
        {

            var refNum = dbContext.ReferenceNumbers.FirstOrDefault();

            updateRelatedTableColumns(ref model);

            var weekDetail = new WeekDetail(model.DateTimeOut.Value);

            var correctedMC = mcRepo.GetCorrectedMC(model.MC, model.NetWt);

            var newPurchase = new PurchaseTransaction()
            {
                BaleCount = model.BaleCount,
                BaleTypeDesc = baleTypeRepository.Get().Where(a => a.BaleTypeId == model.BaleTypeId).Take(1).Select(a => a.BaleTypeDesc).FirstOrDefault(),
                BaleTypeId = model.BaleTypeId,
                BalingStationNum = model.BalingStationNum,
                BalingStationCode = model.BalingStationCode,
                BalingStationName = model.BalingStationName,
                CategoryDesc = model.CategoryDesc,
                CategoryId = model.CategoryId,
                Corrected10 = correctedMC.Corrected10,
                Corrected12 = correctedMC.Corrected12,
                Corrected14 = correctedMC.Corrected14,
                Corrected15 = correctedMC.Corrected15,
                DateTimeIn = model.DateTimeIn,
                DateTimeOut = model.IsOfflineOut??false ? model.DateTimeIn : DateTime.Now,
                DriverName = model.DriverName?.ToUpper(),
                DRNum = model.DRNum?.ToUpper(),
                FirstDay = weekDetail.FirstDay,
                FactoryWt = model.PlantNetWt,
                GrossWt = model.GrossWt,
                IsOfflineIn = model.IsOfflineIn,
                IsOfflineOut = model.IsOfflineOut??false,
                LastDay = weekDetail.LastDay,
                MC = model.MC,
                MCDate = model.MoistureReaderLogs.FirstOrDefault()?.DTLog,
                MCStatus = model.MCStatus,
                MoistureReaderProcess = model.MoistureReaderProcess,
                MoistureReaderDesc = model.MoistureReaderDesc,
                MoistureReaderId = model.MoistureReaderId,
                MoistureReaderLogs = model.MoistureReaderLogs,
                MoistureSettingsId = 1,
                OT = model.OT,
                NetWt = model.NetWt,
                PM = model.PM,
                PurchaseOrderId = model.PurchaseOrderId,
                PONum = model.PONum,
                POType = model.POType,
                Price = model.Price,
                PrintCount = 0,
                RawMaterialDesc = model.CommodityDesc,
                RawMaterialId = model.CommodityId,
                ReceiptNum = refNum.PurchaseReceiptNum,
                Remarks = model.Remarks?.ToUpper(),
                SignatoryId = 1,
                SourceCategoryDesc = model.SourceCategoryDesc,
                SourceCategoryId = model.SourceCategoryId,
                SourceId = model.SourceId,
                SourceName = model.SourceName,
                SupplierId = model.ClientId,
                SupplierName = model.ClientName,
                SubSupplierName = model.SubSupplierName?.ToUpper(),
                TareWt = model.TareWt,
                TimeZoneIn = model.DateTimeIn.GetTimeZone(),
                TimeZoneOut = model.DateTimeOut.GetTimeZone(),
                Trip = model.Trip?.ToUpper(),
                VehicleNum = model.VehicleNum?.ToUpper(),
                VehicleTypeCode = model.VehicleTypeCode,
                VehicleTypeId = model.VehicleTypeId,
                WeekDay = weekDetail.WeekDay,
                WeekNum = weekDetail.WeekNum,
                WeigherInId = model.WeigherInId,
                WeigherInName = model.WeigherInName,
                WeigherOutId =model.WeigherOutId,
                WeigherOutName = model.WeigherOutName
            };

            dbContext.PurchaseTransactions.Add(newPurchase);

            if (subSupplierRepository.Get().Count(a => a.SubSupplierName == model.SubSupplierName) == 0)
            {
                dbContext.SubSuppliers.Add(new SubSupplier() { SubSupplierName = model.SubSupplierName?.Trim(), IsActive = true });
            }

            refNum.PurchaseReceiptNum = String.Format(StringFormats.REFNO_FORMAT, Convert.ToInt32(refNum.PurchaseReceiptNum) + 1);
            dbContext.ReferenceNumbers.Update(refNum);

            dbContext.Inyards.Remove(model);

            dbContext.SaveChanges();

            if (model.IsOfflineOut??false)
            {
                var auditLog = new AuditLog()
                {
                    AuditLogEventId = auditLogEventRepository.GetOfflineOutEventId(),
                    UserAccountId = model.WeigherInId,
                    Notes = formatPurchaseOfflineOutEvent(newPurchase)
                };
                auditLogRepository.Create(auditLog);
            }

            balingStationRepository.CheckAndCreateStockStatusReminder();

            return newPurchase;
        }

        public SaleTransaction WeighoutSale(Inyard model)
        {
            model.DateTimeOut = model.IsOfflineOut??false ? model.DateTimeIn : DateTime.Now;
            var refNum = dbContext.ReferenceNumbers.FirstOrDefault();

            updateRelatedTableColumns(ref model);

            var weekDetail = new WeekDetail(model.DateTimeOut.Value);

            var correctedMC = mcRepo.GetCorrectedMC(model.MC, model.NetWt);

            var saleTransaction = new SaleTransaction()
            {
                BaleCount = model.BaleCount,
                BaleTypeDesc = baleTypeRepository.Get().Where(a => a.BaleTypeId == model.BaleTypeId).Take(1).Select(a => a.BaleTypeDesc).FirstOrDefault(),
                BaleTypeId = model.BaleTypeId,
                BalingStationCode = model.BalingStationCode,
                BalingStationName = model.BalingStationName,
                BalingStationNum = model.BalingStationNum,
                CategoryDesc = model.CategoryDesc,
                CategoryId = model.CategoryId,
                Corrected10 = correctedMC.Corrected10,
                Corrected12 = correctedMC.Corrected12,
                Corrected14 = correctedMC.Corrected14,
                Corrected15 = correctedMC.Corrected15,
                CustomerId = model.ClientId,
                CustomerName = model.ClientName,
                DateTimeIn = model.DateTimeIn,
                DateTimeOut = model.IsOfflineOut ?? false ? model.DateTimeIn : DateTime.Now,
                DriverName = model.DriverName??String.Empty.ToUpper(),
                FirstDay = weekDetail.FirstDay,
                GrossWt = model.GrossWt,
                HaulerId = model.HaulerId??0,
                HaulerName = model.HaulerName,
                IsOfflineIn = model.IsOfflineIn,
                IsOfflineOut = model.IsOfflineOut ?? false,
                LastDay = weekDetail.LastDay,
                MC = model.MC,
                MCStatus = model.MCStatus,
                MoistureReaderProcess = model.MoistureReaderProcess,
                MoistureReaderDesc = model.MoistureReaderDesc,
                MoistureReaderId = model.MoistureReaderId,
                MoistureSettingsId = 1,
                OT = model.OT,
                NetWt = model.NetWt,
                PM = model.PM,
                Price = model.Price,
                PrintCount = 0,
                ProductId = model.CommodityId,
                ProductDesc = model.CommodityDesc,
                ReceiptNum = refNum.SaleReceiptNum,
                Remarks = model.Remarks??String.Empty.ToUpper(),
                SealNum = model.SealNum,
                SaleBales = model.Bales.Select(a=>new SaleBale() { BaleId = a.BaleId}).ToList(),
                SignatoryId = 1,
                TareWt = model.TareWt,
                TimeZoneIn = model.DateTimeIn.GetTimeZone(),
                TimeZoneOut = model.DateTimeOut.GetTimeZone(),
                Trip = model.Trip?.ToUpper(),
                VehicleNum = model.VehicleNum?.ToUpper(),
                VehicleTypeCode = model.VehicleTypeCode,
                VehicleTypeId = model.VehicleTypeId,
                WeekDay = weekDetail.WeekDay,
                WeekNum = weekDetail.WeekNum,
                WeigherInId = model.WeigherInId,
                WeigherInName = model.WeigherInName,
                WeigherOutId = model.WeigherOutId,
                WeigherOutName = model.WeigherOutName

            };

            using var transaction = dbContext.Database.BeginTransaction();

            dbContext.SaleTransactions.Add(saleTransaction);

            refNum.SaleReceiptNum = String.Format(StringFormats.REFNO_FORMAT, Convert.ToInt32(refNum.SaleReceiptNum) + 1);
            dbContext.ReferenceNumbers.Update(refNum);

            dbContext.Inyards.Remove(model);

            dbContext.SaveChanges();

            transaction.Commit();

            if (model.IsOfflineOut ?? false)
            {
                var auditLog = new AuditLog()
                {
                    AuditLogEventId = auditLogEventRepository.GetOfflineOutEventId(),
                    UserAccountId = model.WeigherInId,
                    Notes = formatSalefflineOutEvent(saleTransaction)
                };
                auditLogRepository.Create(auditLog);
            }


            baleRepository.CheckAndCreateBaleOverageReminder();
            balingStationRepository.CheckAndCreateStockStatusReminder();

            return saleTransaction;
        }

        private string getUpdateBalesQuery(SaleTransaction sale, List<Bale> bales)
        {
            if (bales == null || bales.Count() == 0) return string.Empty;
            var baleIds = bales.Select(a => a.BaleId).ToArray();
            var str = new StringBuilder();
            str.AppendLine($"UPDATE BALES SET SaleId = '{sale.SaleId}',DTDelivered = '{sale.DateTimeOut}'");
            str.AppendLine($"WHERE BaleId in ({String.Join(",", baleIds)})");
            return str.ToString();
        }

        public bool Delete(Inyard model)
        {
            dbContext.Inyards.Remove(model);

            vehicleDeliveryRestrictionRepository.Delete(new VehicleDeliveryRestriction(model.VehicleNum, model.CommodityId)
            {
                DateTimeIn = model.DateTimeIn,
            });

            dbContext.SaveChanges();
            return true;
        }

        public bool BulkDelete(string[] id)
        {
            if (id == null) return false;
            if (id.Length == 0) return false;

            var entitiesToDelete = dbContext.Inyards.Where(a => id.Contains(a.InyardId.ToString()));

            dbContext.Inyards.RemoveRange(entitiesToDelete);
            dbContext.SaveChanges();
            return true;
        }

        public IQueryable<Inyard> Get(Inyard model)
        {
            return dbContext.Inyards.AsNoTracking();
        }

        public Inyard GetById(long id)
        {
            return dbContext.Inyards.Find(id);
        }

        public Inyard GetByReceiptNum(string id)
        {
            return dbContext.Inyards.AsNoTracking().First(a => a.InyardNum == id);
        }

        public Inyard Update(Inyard model)
        {
            var entity = dbContext.Inyards.Find(model.InyardId);
            if (entity == null)
            {
                throw new Exception("Selected Record does not exists.");
            }

            updateRelatedTableColumns(ref model);

            var oldVehicleNum = entity.VehicleNum;
            var oldCommodityId = entity.CommodityId;

            entity.ClientId = model.ClientId;
            entity.ClientName = model.ClientName?.ToUpper();
            entity.CommodityId = model.CommodityId;
            entity.CommodityDesc = model.CommodityDesc;
            entity.BaleCount = model.BaleCount;
            entity.BaleTypeId = model.BaleTypeId;
            entity.HaulerId = model.HaulerId;
            entity.HaulerName = model.HaulerName;
            entity.PurchaseOrderId = model.PurchaseOrderId;
            entity.PONum = model.PONum;
            entity.POType = model.POType;
            entity.Price = model.Price;
            entity.DriverName = model.DriverName?.ToUpper();
            entity.DRNum = model.DRNum?.ToUpper(); ;
            entity.PlantTruckOrigin = model.PlantTruckOrigin?.ToUpper();
            entity.Remarks = model.Remarks?.ToUpper();
            entity.SealNum = model.SealNum?.ToUpper();
            entity.SubSupplierName = model.SubSupplierName?.ToUpper();
            entity.TimeZoneIn = model.TimeZoneIn?.ToUpper();
            entity.Trip = model.Trip?.ToUpper();
            entity.VehicleNum = model.VehicleNum?.ToUpper();
            entity.WeigherInId = model.WeigherInId;
            entity.WeigherInName = model.WeigherInName;

            dbContext.Inyards.Update(entity);
            dbContext.SaveChanges();
            dbContext.Entry<Inyard>(entity).State = EntityState.Detached;

            if (model.IsOfflineIn == false)
            {
                if (oldVehicleNum != model.VehicleNum || oldCommodityId != model.CommodityId)
                {
                    var oldVd = new VehicleDeliveryRestriction(oldVehicleNum, oldCommodityId);
                    var newVD = new VehicleDeliveryRestriction(model.VehicleNum, model.CommodityId);
                    vehicleDeliveryRestrictionRepository.Update(oldVd, newVD);
                }

                if (oldVehicleNum != model.VehicleNum)
                {
                    var oldpg = new PurchaseGrossWtRestriction(oldVehicleNum, model.InitialWt);
                    var newpg = new PurchaseGrossWtRestriction(model.VehicleNum, model.InitialWt);
                    purchaseGrossWtRestrictionRepository.Update(oldpg, newpg);
                }
            }

            return entity;
        }

        public SqlRawParameter GetSqlRawParameter(Inyard parameters)
        {
            if (parameters == null) return new SqlRawParameter();
            var sqlQry = new StringBuilder();
            sqlQry.AppendLine("SELECT * FROM Inyards");
            var whereClauses = new List<string>();
            var sqlParams = new List<SqlParameter>();
            if (!parameters.InyardNum.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.InyardNum).Parametarize(), parameters.InyardNum));
                whereClauses.Add($"{nameof(parameters.InyardNum)} = {nameof(parameters.InyardNum).Parametarize()}");
            }
            if (!parameters.VehicleNum.IsNull())
            {
                sqlParams.Add(new SqlParameter(nameof(parameters.VehicleNum).Parametarize(), parameters.VehicleNum));
                whereClauses.Add($"{nameof(parameters.VehicleNum)} = {nameof(parameters.VehicleNum).Parametarize()}");
            }
            if (whereClauses.Count > 0)
            {
                sqlQry.AppendLine(" WHERE ");
                sqlQry.AppendLine(String.Join(" AND ", whereClauses.ToArray()));
            }

            return new SqlRawParameter() { SqlParameters = sqlParams, SqlQuery = sqlQry.ToString() };
        }

        public void PrintReceipt(PrintReceiptModel model)

        {
            throw new NotImplementedException();
        }

        DataSet ITransDbRepository<Inyard>.PrintReceipt(PrintReceiptModel model) => throw new NotImplementedException();

        public Inyard updateRelatedTableColumns(ref Inyard outModifiedInyard)
        {
            var vehicleNum = outModifiedInyard.VehicleNum;
            var vehicle = vehicleRepository.Get()
    .Include(a => a.VehicleType).DefaultIfEmpty()
    .Where(a => a.VehicleNum == vehicleNum)
    .Select(a => new { a.VehicleNum, a.VehicleTypeId, VehicleTypeCode = a.VehicleType == null ? "" : a.VehicleType.VehicleTypeCode }).ToList().FirstOrDefault();
            outModifiedInyard.VehicleTypeId = vehicle?.VehicleTypeId ?? 0;
            outModifiedInyard.VehicleTypeCode = vehicle?.VehicleTypeCode;

            if (outModifiedInyard.TransactionTypeCode == "I")
            {
                var clientId = outModifiedInyard.ClientId;
                outModifiedInyard.ClientName = supplierRepository.Get()
                 .Where(a => a.SupplierId == clientId).Select(a => a.SupplierName).FirstOrDefault();

                var commodityId = outModifiedInyard.CommodityId;
                var material = rawMaterialRepository.Get()
                    .Where(a => a.RawMaterialId == commodityId)
                    .Include(a => a.Category).DefaultIfEmpty()
                    .Select(a => new { a.RawMaterialDesc, a.Price,a.CategoryId, CategoryDesc = a.Category == null ? null : a.Category.CategoryDesc })
                    .FirstOrDefault();
                var poNum = outModifiedInyard.PONum;

                outModifiedInyard.CommodityDesc = material?.RawMaterialDesc;
                outModifiedInyard.CategoryId = material?.CategoryId ?? 0;
                outModifiedInyard.CategoryDesc = material?.CategoryDesc;
                outModifiedInyard.Price = material?.Price ?? 0;

                var purchaseOrderId = outModifiedInyard.PurchaseOrderId;
                var poDetails = purchaseOrderRepository.Get()
        .Where(a => a.PurchaseOrderId == purchaseOrderId).Select(a => new { a.PONum, a.Price, a.POType }).FirstOrDefault();

                outModifiedInyard.PONum = poDetails?.PONum ?? String.Empty;
                outModifiedInyard.Price = poDetails?.Price ?? 0;
                outModifiedInyard.POType = poDetails?.POType;

                var sourceId = outModifiedInyard.SourceId;
                var source = sourceRepository.Get()
                    .Where(a => a.SourceId == sourceId)
                    .Include(a => a.SourceCategory).DefaultIfEmpty()
                    .Select(a => new { a.SourceDesc, a.SourceCategoryId, SourceCategoryDesc = a.SourceCategory == null ? null : a.SourceCategory.Description })
                    .FirstOrDefault();
                outModifiedInyard.SourceName = source?.SourceDesc;
                outModifiedInyard.SourceCategoryId = source?.SourceCategoryId ?? 0;
                outModifiedInyard.SourceCategoryDesc = source?.SourceCategoryDesc;

            }
            else
            {
                var clientId = outModifiedInyard.ClientId;
                outModifiedInyard.ClientName = customerRepository.Get()
                .Where(a => a.CustomerId == clientId).Select(a => a.CustomerName).FirstOrDefault();

                var haulerId = outModifiedInyard.HaulerId;
                outModifiedInyard.HaulerName = haulerRepository.Get()
                .Where(a => a.HaulerId == haulerId).Select(a => a.HaulerName).FirstOrDefault();

                var commodityId = outModifiedInyard.CommodityId;
                var product = productRepository.Get()
                    .Where(a => a.ProductId == commodityId)
                    .Include(a => a.Category).DefaultIfEmpty()
                    .Select(a => new { a.ProductDesc, a.CategoryId, CategoyDesc = a.Category == null ? null : a.Category.CategoryDesc })
                    .FirstOrDefault();
                outModifiedInyard.CommodityDesc = product?.ProductDesc;
                outModifiedInyard.CategoryId = product?.CategoryId ?? 0;
                outModifiedInyard.CategoryDesc = product?.CategoyDesc;
            }

            var msId = outModifiedInyard.MoistureReaderId;
            outModifiedInyard.MoistureReaderDesc = moistureReaderRepository.Get()
                .Where(a => a.MoistureReaderId == msId).Select(a => a.Description).FirstOrDefault();

            var balingStation = balingStationRepository.Get().Where(a => a.Selected).Take(1).AsNoTracking()
                .Select(a => new { a.BalingStationNum, a.BalingStationCode, a.BalingStationName }).FirstOrDefault();

            outModifiedInyard.BalingStationNum = balingStation.BalingStationNum;
            outModifiedInyard.BalingStationCode = balingStation.BalingStationCode;
            outModifiedInyard.BalingStationName = balingStation.BalingStationName;

            var userAccountId = String.Empty;
            if (outModifiedInyard.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN ||
                outModifiedInyard.TransactionProcess == SysUtility.Enums.TransactionProcess.UPDATE_WEIGH_IN)
            {
                userAccountId = outModifiedInyard.WeigherInId;
                outModifiedInyard.WeigherInName = userAccountRepository.Get().Where(a => a.UserAccountId == userAccountId)
                .Select(a => a.FullName).FirstOrDefault();

            } else if (outModifiedInyard.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT ||
                outModifiedInyard.TransactionProcess == SysUtility.Enums.TransactionProcess.UPDATE_WEIGH_OUT)
            {
                userAccountId = outModifiedInyard.WeigherOutId;
                outModifiedInyard.WeigherOutName = userAccountRepository.Get().Where(a => a.UserAccountId == userAccountId)
                .Select(a => a.FullName).FirstOrDefault();
            }
            return outModifiedInyard;
        }


        private string formatOfflineInEvent(Inyard model)
        {
            var str = new StringBuilder();
            str.Append($"Reference Number: {model.InyardNum};");
            return str.ToString();
        }

        private string formatPurchaseOfflineOutEvent(PurchaseTransaction model)
        {
            var str = new StringBuilder();
            str.Append($"Reference Number: {model.ReceiptNum};");
            return str.ToString();
        }

        private string formatSalefflineOutEvent(SaleTransaction model)
        {
            var str = new StringBuilder();
            str.Append($"Reference Number: {model.ReceiptNum};");
            return str.ToString();
        }

    }
}
