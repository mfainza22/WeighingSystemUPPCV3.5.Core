using Microsoft.EntityFrameworkCore;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SysUtility;
using SysUtility.Config.Interfaces;
using SysUtility.Extensions;
using SysUtility.Validations.UPPC;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class TransValidationRepository : ITransValidationRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IAppConfigRepository appConfigRepository;
        private readonly IMoistureReaderRepository moistureReaderRepository;
        private readonly IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository;
        private readonly IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IReferenceNumberRepository refNumRepository;
        private readonly IPurchaseTransactionRepository purchaseTransactionRepository;
        private readonly ISaleTransactionRepository saleTransactionRepository;

        public ICustomerRepository CustomerRepository { get; }
        public ISupplierRepository SupplierRepository { get; }
        public IRawMaterialRepository RawMaterialRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IHaulerRepository HaulerRepository { get; }
        public IBaleTypeRepository BaleTypeRepository { get; }
        public ISourceRepository SourceRepository { get; }
        public ISourceCategoryRepository SourceCategoryRepository { get; }
        public ISubSupplierRepository SubSupplierRepository { get; }

        public TransValidationRepository(DatabaseContext dbContext, ICustomerRepository customerRepository, ISupplierRepository supplierRepository,
            IRawMaterialRepository rawMaterialRepository, IProductRepository productRepository, IHaulerRepository haulerRepository,
            IBaleTypeRepository baleTypeRepository,
            ISourceRepository sourceRepository,
            ISourceCategoryRepository sourceCategoryRepository, ISubSupplierRepository subSupplierRepository,
            IAppConfigRepository appConfigRepository,
            IMoistureReaderRepository moistureReaderRepository,
            IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository,
            IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
             IReferenceNumberRepository refNumRepository,
             IPurchaseTransactionRepository purchaseTransactionRepository,
             ISaleTransactionRepository saleTransactionRepository)
        {
            this.dbContext = dbContext;
            CustomerRepository = customerRepository;
            SupplierRepository = supplierRepository;
            RawMaterialRepository = rawMaterialRepository;
            ProductRepository = productRepository;
            HaulerRepository = haulerRepository;
            BaleTypeRepository = baleTypeRepository;
            SourceRepository = sourceRepository;
            SourceCategoryRepository = sourceCategoryRepository;
            SubSupplierRepository = subSupplierRepository;
            this.appConfigRepository = appConfigRepository;
            this.moistureReaderRepository = moistureReaderRepository;
            this.vehicleDeliveryRestrictionRepository = vehicleDeliveryRestrictionRepository;
            this.purchaseGrossWtRestrictionRepository = purchaseGrossWtRestrictionRepository;
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.refNumRepository = refNumRepository;
            this.purchaseTransactionRepository = purchaseTransactionRepository;
            this.saleTransactionRepository = saleTransactionRepository;
        }



        public bool ValidateClient(long clientId, string transTypeCode)
        {
            if (transTypeCode == "I")
            {
                var supCount = dbContext.Suppliers.AsNoTracking().Count(a => a.SupplierId == clientId);
                return supCount > 0;
            }
            else
            {
                var custCount = dbContext.Customers.AsNoTracking().Count(a => a.CustomerId == clientId);
                return custCount > 0;
            }
        }

        public bool ValidateCommodity(long commodityId, string transTypeCode)
        {
            if (commodityId == 0) return true;
            if (transTypeCode == "I")
            {
                var count = dbContext.RawMaterials.AsNoTracking().Count(a => a.RawMaterialId == commodityId);
                return count > 0;
            }
            else
            {
                var count = dbContext.Products.AsNoTracking().Count(a => a.ProductId == commodityId);
                return count > 0;
            }
        }
        public bool CustomerExists(long id)
        {
            return CustomerRepository.Get().Count(a => a.CustomerId == id) > 0;
        }
        public bool SupplierExists(long id)
        {
            return CustomerRepository.Get().Count(a => a.CustomerId == id) > 0;
        }
        public bool RawMaterialExists(long id)
        {
            return RawMaterialRepository.Get().Count(a => a.RawMaterialId == id) > 0;
        }
        public bool ProductExists(long id)
        {
            return ProductRepository.Get().Count(a => a.ProductId == id) > 0;
        }
        public bool HaulerExists(long id)
        {
            return HaulerRepository.Get().Count(a => a.HaulerId == id) > 0;
        }

        public Nullable<DateTime> CheckVehicleDeliveryRestrictionPeriod(string vehicleNum, long commodityId)
        {
            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return null;

            var vehicleDeliveryRestrictionPeriod = appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestrictionPeriod;

            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
            {
                sqlConn.Open();
                var sqlQuery = $@"DECLARE @VehicleNum varchar(20); SET @VehicleNum = '{vehicleNum}'
	                        DECLARE @CommodityId bigint; SET @CommodityId = {commodityId}
	                        DECLARE @SameVehicleLockPeriod int; SET @SameVehicleLockPeriod = {vehicleDeliveryRestrictionPeriod}
	                        DECLARE @LockPeriod DateTime; set @LockPeriod =(DATEADD(mi,+@SameVehicleLockPeriod,GETDATE()))
	                        DECLARE @currentDate DateTime; 
                            IF ((Select Count(*) from Inyards where VehicleNum = @VehicleNum 
                            and CommodityId = @CommodityId and DateTimeIn > @LockPeriod ) +
                            (Select Count(*) from PurchaseTransactions where VehicleNum = @VehicleNum 
                            and RawMaterialId = @CommodityId and  DateTimeIn >@LockPeriod)
	                            +
                            (Select Count(*) from SaleTransactions where VehicleNum = @VehicleNum 
                            and ProductId = @CommodityId and  DateTimeIn >@LockPeriod) > 0) BEGIN
                            SELECT @LockPeriod END ELSE BEGIN select NULL END";

                var command = new SqlCommand(sqlQuery, sqlConn);
   
                DateTime.TryParse(command.ExecuteScalar().ToString(),out var lockPeriod);
                if (lockPeriod.IsEmpty()) return null;
                return lockPeriod;
            }
        }

        public bool CheckPurchaseGrossWtRestrictionPeriod(string vehicleNum, int weight, int purchaseGrossWtRestrictionPeriod = 0)
        {
            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return true;

            if (purchaseGrossWtRestrictionPeriod == 0) purchaseGrossWtRestrictionPeriod = appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionPeriod;

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                var sqlQuery = $@"DECLARE @VehicleNum varchar(20); SET @VehicleNum = '{vehicleNum}'
	                        DECLARE @grossWt int; SET @grossWt = {weight}
	                        DECLARE @SameVehicleLockPeriod int; SET @SameVehicleLockPeriod = {purchaseGrossWtRestrictionPeriod}
	                        DECLARE @purchaseGrossWtRestrictionPeriod Dat
eTime; set @LockPeriod =(DATEADD(mi,-@SameVehicleLockPeriod,GETDATE()))
	                        DECLARE @currentDate DateTime; 
                            IF ((Select Count(*) from Inyards where VehicleNum = @VehicleNum 
                            and GrossWt = @CommodityId and DateTimeIn > @LockPeriod ) +
                            (Select Count(*) from PurchaseTransactions where VehicleNum = @VehicleNum 
                            and RawMaterialId = @CommodityId and  DateTimeIn >@LockPeriod)
	                            +
                            (Select Count(*) from SaleTransactions where VehicleNum = @VehicleNum 
                            and ProductId = @CommodityId and  DateTimeIn >@LockPeriod) > 0) BEGIN
                            SELECT @LockPeriod END ELSE BEGIN select null END";

                command.CommandText = sqlQuery;
                var result = (int)command.ExecuteScalar();
                return result == 0;
            }
        }

        public Dictionary<string, string> ValidateInyard(Inyard model)
        {


            var modelStateDict = new Dictionary<string, string>();
            if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
            {
                var vehicleDeliverRestriction = new VehicleDeliveryRestriction()
                {
                    VehicleNum = model.VehicleNum,
                    CommodityId = model.CommodityId,
                    DateTimeIn = model.DateTimeIn
                };
                var vehicleDeliveryRestrictionResult = vehicleDeliveryRestrictionRepository.CheckRestriction(vehicleDeliverRestriction);
                if (vehicleDeliveryRestrictionResult != null) modelStateDict.Add(nameof(Inyard.VehicleNum), ValidationMessages.VehicleDeliveryInvalid(vehicleDeliveryRestrictionResult.DTRestriction));

                var purchaseGrossRestrictionresult = new PurchaseGrossWtRestriction()
                {
                    VehicleNum = model.VehicleNum,
                    Weight = model.GrossWt,
                    DateTimeIn = model.DateTimeIn
                };

                var purchaseGrossRestrictionResult = purchaseGrossWtRestrictionRepository.CheckRestriction(purchaseGrossRestrictionresult);
                if (purchaseGrossRestrictionResult != null) modelStateDict.Add(nameof(Inyard.GrossWt), ValidationMessages.PurchaseGrossInvalid(purchaseGrossRestrictionResult.DTRestriction));
            }

            if (model.TransactionTypeCode == "I")
            {
                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
                {
                    if (model.GrossWt == 0) modelStateDict.Add(nameof(model.GrossWt), ValidationMessages.InvalidWeight);
                }
                else if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
                {
                    if (model.MC == 0)
                    {
                        modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("MC"));
                    }
                    if (model.TareWt == 0) modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight);
                    if (model.NetWt == 0) modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight);

                    var receiptNum = refNumRepository.Get().FirstOrDefault().PurchaseReceiptNum;
                    if (dbContext.PurchaseTransactions.AsNoTracking().Count(a => a.ReceiptNum == receiptNum) > 0)
                        modelStateDict.Add(nameof(model.InyardNum), ValidationMessages.InvalidReceiptNum);
                    
                }

                var supplier = SupplierRepository.GetById(model.ClientId);
                if (supplier == null) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.SupplierNotExists);
                else model.ClientName = supplier.SupplierName;
                supplier = null;

                var material = RawMaterialRepository.GetById(model.CommodityId);
                if (material == null)
                {
                    modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.RawMaterialNotExists);
                }
                else
                {
                    model.CommodityDesc = material.RawMaterialDesc;
                    model.CategoryId = material.CategoryId;
                }

                if (model.SourceId.IsNullOrZero())
                {
                    modelStateDict.Add(nameof(model.ClientId), "Source is required");
                }
                else
                {
                    var source = SourceRepository.GetById(model.SourceId);
                    if (source == null) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.SourceNotExists);
                    else { 
                        model.SourceName = source.SourceDesc;
                        model.SourceCategoryId = source.SourceCategoryId;
                    }
                    source = null;
                }

                if (model.SourceCategoryId != 0)
                {
                    var sourceCat = SourceCategoryRepository.GetById(model.SourceCategoryId);
                    if (sourceCat == null) modelStateDict.Add(nameof(model.SourceCategoryId), ValidationMessages.SourceCatNotExists);
                    else model.SourceCategoryDesc = sourceCat.Description;
                    sourceCat = null;
                }

                var po = purchaseOrderRepository.ValidatePO(new PurchaseOrder() { PONum = model.PONum });
                if (po == null) modelStateDict.Add(nameof(model.PONum),ValidationMessages.POInvalid);
                else if(po.BalanceRemainingKg < -5000) modelStateDict.Add(nameof(model.PONum), ValidationMessages.PORemainingBalanceInvalid);

            }
            if (model.TransactionTypeCode == "O")
            {
                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
                {
                    if (model.TareWt == 0) modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight);
                }
                else if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
                {
                    if (model.MC == 0)
                    {
                        modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("MC"));
                    }
                    if (model.GrossWt == 0) modelStateDict.Add(nameof(model.GrossWt), ValidationMessages.InvalidWeight);
                    if (model.NetWt == 0) modelStateDict.Add(nameof(model.NetWt), ValidationMessages.InvalidWeight);
                    var receiptNum = refNumRepository.Get().FirstOrDefault().PurchaseReceiptNum;
                    if (dbContext.SaleTransactions.AsNoTracking().Count(a => a.ReceiptNum == receiptNum) > 0)
                        modelStateDict.Add(nameof(model.InyardNum), ValidationMessages.InvalidReceiptNum);
                    if (model.BaleCount == 0)
                    {
                        modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("Bale Count"));
                    }
                }

                var customer = CustomerRepository.GetById(model.ClientId);
                if (customer == null) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.CustomerNotExists);
                else model.ClientName = customer.CustomerName;
                customer = null;

                var product = ProductRepository.GetById(model.CommodityId);
                if (product == null) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.ProductNotExists);
                else model.CommodityDesc = product.ProductDesc;
                product = null;

                if (model.HaulerId.GetValueOrDefault(0) > 0)
                {
                    var hauler = HaulerRepository.GetById(model.HaulerId ?? 0);
                    if (hauler == null) modelStateDict.Add(nameof(model.HaulerId), ValidationMessages.HaulerNotExists);
                    else model.BaleTypeDesc = hauler.HaulerName;
                    hauler = null;
                }

                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
                {
                    if (model.Bales.Count() > 0)
                    {
                        var unRelatedBalesCount = model.Bales.Count(a => a.ProductId != model.CommodityId);
                        if (unRelatedBalesCount > 0)
                        {
                            modelStateDict.Add(nameof(model.CommodityId), "Selected bales must have the product type.");
                        };
                    }
                }
            }

            var baleType = BaleTypeRepository.GetById(model.BaleTypeId);
            if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
            else model.BaleTypeDesc = baleType.BaleTypeDesc;
            baleType = null;

            if ((model.MoistureReaderId??0) > 0)
            {
                var moistureReader = moistureReaderRepository.GetById(model.MoistureReaderId.Value);
                if (moistureReader == null) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
                else model.MoistureReaderDesc= moistureReader.Description;
                moistureReader = null;
            }
   
            return modelStateDict;
        }

        public Dictionary<string, string> ValidatePurchase(PurchaseTransaction model)
        {
            var modelStateDict = new Dictionary<string, string>();

            var supplier = SupplierRepository.GetById(model.SupplierId);
            if (supplier == null) modelStateDict.Add(nameof(model.SupplierId), ValidationMessages.SupplierNotExists);
            else model.SupplierName = supplier.SupplierName;
            supplier = null;

            var material = RawMaterialRepository.GetById(model.RawMaterialId);
            if (material == null)
            {
                modelStateDict.Add(nameof(model.RawMaterialId), ValidationMessages.RawMaterialNotExists);
            }
            else
            {
                model.RawMaterialDesc = material.RawMaterialDesc;
                model.CategoryId = material.CategoryId;
            }

            var baleType = BaleTypeRepository.GetById(model.BaleTypeId);
            if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
            else model.BaleTypeDesc = baleType.BaleTypeDesc;
            baleType = null;

            var source = SourceRepository.GetById(model.SourceId);
            if (source == null) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.SourceNotExists);
            else model.SourceName = source.SourceDesc;
            source = null;

            if (model.SourceCategoryId != 0)
            {
                var sourceCat = SourceCategoryRepository.GetById(model.SourceCategoryId);
                if (sourceCat == null) modelStateDict.Add(nameof(model.SourceCategoryId), ValidationMessages.SourceCatNotExists);
                else model.SourceCategoryDesc = sourceCat.Description;
                sourceCat = null;

            }

            if ((model.MoistureReaderId ?? 0) > 0)
            {
                var moistureReader = moistureReaderRepository.GetById(model.MoistureReaderId.Value);
                if (moistureReader == null) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
                else model.MoistureReaderDesc = moistureReader.Description;
                moistureReader = null;
            }

            return modelStateDict;
        }

        public Dictionary<string, string> ValidateSale(SaleTransaction model)
        {
            var modelStateDict = new Dictionary<string, string>();

            var customer = CustomerRepository.GetById(model.CustomerId);
            if (customer == null) modelStateDict.Add(nameof(model.CustomerId), ValidationMessages.SupplierNotExists);
            else model.CustomerName = customer.CustomerName;
            customer = null;


            var hauler = HaulerRepository.GetById(model.HaulerId);
            if (hauler == null) modelStateDict.Add(nameof(model.HaulerId), ValidationMessages.RawMaterialNotExists);
            else model.HaulerName = hauler.HaulerName;
            hauler = null;

            var product = ProductRepository.GetById(model.ProductId);
            if (product == null) { modelStateDict.Add(nameof(model.ProductId), ValidationMessages.RawMaterialNotExists); }
            else
            {
                model.ProductDesc = product.ProductDesc;
                model.CategoryId = product.CategoryId ?? 0;
            }
            product = null;

            var baleType = BaleTypeRepository.GetById(model.BaleTypeId);
            if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
            else model.BaleTypeDesc = baleType.BaleTypeDesc;
            baleType = null;

            if ((model.MoistureReaderId ?? 0) > 0)
            {
                var moistureReader = moistureReaderRepository.GetById(model.MoistureReaderId.Value);
                if (moistureReader == null) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
                else model.MoistureReaderDesc = moistureReader.Description;
                moistureReader = null;
            }

            return modelStateDict;
        }

    }
}
