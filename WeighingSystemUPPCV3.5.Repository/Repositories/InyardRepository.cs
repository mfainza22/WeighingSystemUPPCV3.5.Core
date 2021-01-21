
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
        private readonly ISubSupplierRepository subSupplierRepository;
        private readonly IMoistureSettingsRepository mcRepo;
        private readonly IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository;
        private readonly IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository;

        public InyardRepository(DatabaseContext dbContext, IReferenceNumberRepository refNumRepository, ISubSupplierRepository subSupplierRepository, IMoistureSettingsRepository mcRepo, IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository, IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository)
        {
            this.dbContext = dbContext;
            this.subSupplierRepository = subSupplierRepository;
            this.mcRepo = mcRepo;
            this.purchaseGrossWtRestrictionRepository = purchaseGrossWtRestrictionRepository;
            this.vehicleDeliveryRestrictionRepository = vehicleDeliveryRestrictionRepository;
        }

        public Inyard WeighIn(Inyard model)
        {
            var refNum = dbContext.ReferenceNumbers.FirstOrDefault();
            model.InyardNum = refNum.InyardNum;

            model.DriverName = model.DriverName?.ToUpper();
            //model.DateTimeIn = model.IsOfflineIn ? model.DateTimeIn : DateTime.Now;
            model.TimeZoneIn = model.DateTimeIn.GetTimeZone();
            model.DRNum = model.DRNum?.ToUpper();
            model.PlantTruckOrigin ??= String.Empty.ToUpper();
            model.Remarks ??= String.Empty.ToUpper();
            model.SealNum = model.SealNum?.ToUpper();
            model.SubSupplierName = model.SubSupplierName?.ToUpper();
            model.TimeZoneIn = model.TimeZoneIn?.ToUpper();
            model.Trip = model.Trip?.ToUpper();
            model.VehicleNum = model.VehicleNum?.ToUpper();
           
            dbContext.Inyards.Add(model);

            refNum.InyardNum = String.Format(StringFormats.REFNO_FORMAT, Convert.ToInt32(refNum.InyardNum) + 1);
            dbContext.ReferenceNumbers.Update(refNum);
            dbContext.SaveChanges();

            if (model.IsOfflineIn == false)
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

            return model;
        }

        public PurchaseTransaction WeighoutPurchase(Inyard model)
        {
            model.DateTimeOut = model.IsOfflineOut ? model.DateTimeIn : DateTime.Now;
            model.TimeZoneOut = model.DateTimeOut.GetTimeZone();
            var refNum = dbContext.ReferenceNumbers.FirstOrDefault();

            var purchase = new PurchaseTransaction();
            ((IInyard)model).CopyPropertiesTo<IInyard>(purchase);

            purchase.ReceiptNum = refNum.PurchaseReceiptNum;
            purchase.SupplierId = model.ClientId;
            purchase.SupplierName = model.ClientName;
            purchase.RawMaterialId = model.CommodityId;
            purchase.RawMaterialDesc = model.CommodityDesc;
            purchase.PONum = model.PONum?.ToUpper();
            purchase.DRNum = model.DRNum?.ToUpper();
            purchase.SubSupplierName = model.SubSupplierName?.ToUpper();
            purchase.SourceId = model.SourceId;
            purchase.SourceCategoryId = model.SourceCategoryId;
            purchase.Price = model.Price;
            purchase.SourceId = model.SourceId;
            purchase.SourceName = model.SourceName;
            purchase.SourceCategoryId = model.SourceCategoryId;
            purchase.SourceCategoryDesc = model.SourceCategoryDesc;
            purchase.MoistureReaderId = model.MoistureReaderId;

            var weekDetail = new WeekDetail(model.DateTimeOut.Value);
            purchase.WeekDay = weekDetail.WeekDay;
            purchase.WeekNum = weekDetail.WeekNum;
            purchase.FirstDay = weekDetail.FirstDay;
            purchase.LastDay = weekDetail.LastDay;

            var correctedMC = mcRepo.GetCorrectedMC(model.MC, model.NetWt);
            purchase.MCStatus = correctedMC.MCStatus;
            purchase.Corrected10 = correctedMC.Corrected10;
            purchase.Corrected12 = correctedMC.Corrected12;
            purchase.Corrected14 = correctedMC.Corrected14;
            purchase.Corrected15 = correctedMC.Corrected15;


            dbContext.PurchaseTransactions.Add(purchase);

            if (subSupplierRepository.Get().Count(a => a.SubSupplierName == model.SubSupplierName) == 0)
            {
                dbContext.SubSuppliers.Add(new SubSupplier() { SubSupplierName = model.SubSupplierName?.Trim(), IsActive = true });
            }

            refNum.PurchaseReceiptNum = String.Format(StringFormats.REFNO_FORMAT, Convert.ToInt32(refNum.PurchaseReceiptNum) + 1);
            dbContext.ReferenceNumbers.Update(refNum);
            
            dbContext.Inyards.Remove(model);

            dbContext.SaveChanges();

            for (var i = 0; i <= model.MoistureReaderLogs.Count() - 1; i++)
            {
                model.MoistureReaderLogs[i].TransactionId = purchase.PurchaseId;
            }

            dbContext.moistureReaderLogs.AddRange(model.MoistureReaderLogs);

            dbContext.SaveChanges();
            return purchase;
        }

        public SaleTransaction WeighoutSale(Inyard model)
        {
            model.DateTimeOut = model.IsOfflineOut ? model.DateTimeIn : DateTime.Now;
            var refNum = dbContext.ReferenceNumbers.FirstOrDefault();

            var sale = new SaleTransaction();
            ((IInyard)model).CopyPropertiesTo<IInyard>(sale);

            sale.ReceiptNum = refNum.SaleReceiptNum;
            sale.CustomerId = model.ClientId;
            sale.CustomerName = model.ClientName;
            sale.ProductId = model.CommodityId;
            sale.ProductDesc = model.CommodityDesc;
            sale.HaulerId = model.HaulerId ?? 0;
            sale.HaulerName = model.HaulerName;
            sale.SealNum = model.SealNum;

            var weekDetail = new WeekDetail(model.DateTimeOut.Value);
            sale.WeekDay = weekDetail.WeekDay;
            sale.WeekNum = weekDetail.WeekNum;
            sale.FirstDay = weekDetail.FirstDay;
            sale.LastDay = weekDetail.LastDay;

            var correctedMC = mcRepo.GetCorrectedMC(model.MC, model.NetWt);
            var mcSettings = mcRepo.Get().Select(a => a).First();
            sale.MCStatus = 0;
            sale.Corrected10 = correctedMC.Corrected10;
            sale.Corrected12 = correctedMC.Corrected12;
            sale.Corrected14 = correctedMC.Corrected14;
            sale.Corrected15 = correctedMC.Corrected15;

            dbContext.SaleTransactions.Add(sale);

            refNum.SaleReceiptNum = String.Format(StringFormats.REFNO_FORMAT, Convert.ToInt32(refNum.SaleReceiptNum) + 1);
            dbContext.ReferenceNumbers.Update(refNum);

            dbContext.Inyards.Remove(model);

            dbContext.SaveChanges();

            var str = getUpdateBalesQuery(sale, model.Bales);
            if (str != String.Empty) dbContext.Database.ExecuteSqlRaw(str);

            return sale;
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

            entity.DriverName = model.DriverName?.ToUpper();
            entity.DRNum = model.DRNum?.ToUpper(); ;
            entity.PlantTruckOrigin = model.PlantTruckOrigin?.ToUpper();
            entity.Remarks = model.Remarks?.ToUpper();
            entity.SealNum = model.SealNum?.ToUpper();
            entity.SubSupplierName = model.SubSupplierName?.ToUpper();
            entity.TimeZoneIn = model.TimeZoneIn?.ToUpper();
            entity.Trip = model.Trip?.ToUpper();
            entity.VehicleNum = model.VehicleNum?.ToUpper();

            dbContext.Inyards.Update(entity);
            dbContext.SaveChanges();

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
       
    }
}
