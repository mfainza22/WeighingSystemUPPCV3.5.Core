//using Microsoft.EntityFrameworkCore;
//using WeighingSystemUPPCV3_5_Repository.IRepositories;
//using WeighingSystemUPPCV3_5_Repository.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using SysUtility;
//using SysUtility.Config.Interfaces;
//using SysUtility.Extensions;
//using SysUtility.Validations.UPPC;
//using WeighingSystemUPPCV3_5_Repository.Interfaces;

//namespace WeighingSystemUPPCV3_5_Repository.Repositories
//{
//    public class TransValidationRepository : ITransValidationRepository
//    {
//        private readonly DatabaseContext dbContext;
//        private readonly ICustomerRepository customerRepository;
//        private readonly ISupplierRepository supplierRepository;
//        private readonly IAppConfigRepository appConfigRepository;
//        private readonly IMoistureReaderRepository moistureReaderRepository;
//        private readonly IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository;
//        private readonly IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository;
//        private readonly IPurchaseOrderRepository purchaseOrderRepository;
//        private readonly IReferenceNumberRepository refNumRepository;
//        private readonly IPurchaseTransactionRepository purchaseTransactionRepository;
//        private readonly ISaleTransactionRepository saleTransactionRepository;
//        private readonly IVehicleRepository vehicleRepository;
//        private readonly ICategoryRepository categoryRepository;
//        private readonly IProductRepository productRepository;
//        private readonly IHaulerRepository haulerRepository;
//        private readonly IBaleTypeRepository baleTypeRepository;
//        private readonly ISourceRepository sourceRepository;
//        private readonly ISourceCategoryRepository sourceCategoryRepository;
//        private readonly ISubSupplierRepository subSupplierRepository;
//        private readonly IRawMaterialRepository rawMaterialRepository;


//        public TransValidationRepository(DatabaseContext dbContext,
//            ICustomerRepository customerRepository,
//            ISupplierRepository supplierRepository,
//            IRawMaterialRepository rawMaterialRepository,
//            ICategoryRepository categoryRepository,
//            IProductRepository productRepository,
//            IHaulerRepository haulerRepository,
//            IBaleTypeRepository baleTypeRepository,
//            ISourceRepository sourceRepository,
//            ISourceCategoryRepository sourceCategoryRepository,
//            ISubSupplierRepository subSupplierRepository,
//            IAppConfigRepository appConfigRepository,
//            IMoistureReaderRepository moistureReaderRepository,
//            IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository,
//            IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository,
//            IPurchaseOrderRepository purchaseOrderRepository,
//             IReferenceNumberRepository refNumRepository,
//             IPurchaseTransactionRepository purchaseTransactionRepository,
//             ISaleTransactionRepository saleTransactionRepository,
//             IVehicleRepository vehicleRepository)
//        {
//            this.dbContext = dbContext;
//            this.customerRepository = customerRepository;
//            this.supplierRepository = supplierRepository;
//            this.appConfigRepository = appConfigRepository;
//            this.moistureReaderRepository = moistureReaderRepository;
//            this.vehicleDeliveryRestrictionRepository = vehicleDeliveryRestrictionRepository;
//            this.purchaseGrossWtRestrictionRepository = purchaseGrossWtRestrictionRepository;
//            this.purchaseOrderRepository = purchaseOrderRepository;
//            this.refNumRepository = refNumRepository;
//            this.purchaseTransactionRepository = purchaseTransactionRepository;
//            this.saleTransactionRepository = saleTransactionRepository;
//            this.vehicleRepository = vehicleRepository;
//            this.categoryRepository = categoryRepository;
//            this.productRepository = productRepository;
//            this.haulerRepository = haulerRepository;
//            this.baleTypeRepository = baleTypeRepository;
//            this.sourceRepository = sourceRepository;
//            this.sourceCategoryRepository = sourceCategoryRepository;
//            this.subSupplierRepository = subSupplierRepository;
//            this.rawMaterialRepository = rawMaterialRepository;
//        }



//        public bool ValidateClient(long clientId, string transTypeCode)
//        {
//            if (transTypeCode == "I")
//            {
//                var supCount = dbContext.Suppliers.AsNoTracking().Count(a => a.SupplierId == clientId);
//                return supCount > 0;
//            }
//            else
//            {
//                var custCount = dbContext.Customers.AsNoTracking().Count(a => a.CustomerId == clientId);
//                return custCount > 0;
//            }
//        }

//        public bool ValidateCommodity(long commodityId, string transTypeCode)
//        {
//            if (commodityId == 0) return true;
//            if (transTypeCode == "I")
//            {
//                var count = dbContext.RawMaterials.AsNoTracking().Count(a => a.RawMaterialId == commodityId);
//                return count > 0;
//            }
//            else
//            {
//                var count = dbContext.Products.AsNoTracking().Count(a => a.ProductId == commodityId);
//                return count > 0;
//            }
//        }
//        public bool CustomerExists(long id)
//        {
//            return customerRepository.Get().Count(a => a.CustomerId == id) > 0;
//        }
//        public bool SupplierExists(long id)
//        {
//            return supplierRepository.Get().Count(a => a.SupplierId == id) > 0;
//        }
//        public bool RawMaterialExists(long id)
//        {
//            return rawMaterialRepository.Get().Count(a => a.RawMaterialId == id) > 0;
//        }
//        public bool ProductExists(long id)
//        {
//            return productRepository.Get().Count(a => a.ProductId == id) > 0;
//        }
//        public bool HaulerExists(long id)
//        {
//            return haulerRepository.Get().Count(a => a.HaulerId == id) > 0;
//        }

//        public Nullable<DateTime> CheckVehicleDeliveryRestrictionPeriod(string vehicleNum, long commodityId)
//        {
//            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return null;

//            var vehicleDeliveryRestrictionPeriod = appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestrictionPeriod;

//            using (var sqlConn = new SqlConnection(dbContext.Database.GetDbConnection().ConnectionString))
//            {
//                sqlConn.Open();
//                var sqlQuery = $@"DECLARE @VehicleNum varchar(20); SET @VehicleNum = '{vehicleNum}'
//	                        DECLARE @CommodityId bigint; SET @CommodityId = {commodityId}
//	                        DECLARE @SameVehicleLockPeriod int; SET @SameVehicleLockPeriod = {vehicleDeliveryRestrictionPeriod}
//	                        DECLARE @LockPeriod DateTime; set @LockPeriod =(DATEADD(mi,+@SameVehicleLockPeriod,GETDATE()))
//	                        DECLARE @currentDate DateTime; 
//                            IF ((Select Count(*) from Inyards where VehicleNum = @VehicleNum 
//                            and CommodityId = @CommodityId and DateTimeIn > @LockPeriod ) +
//                            (Select Count(*) from PurchaseTransactions where VehicleNum = @VehicleNum 
//                            and RawMaterialId = @CommodityId and  DateTimeIn >@LockPeriod)
//	                            +
//                            (Select Count(*) from SaleTransactions where VehicleNum = @VehicleNum 
//                            and ProductId = @CommodityId and  DateTimeIn >@LockPeriod) > 0) BEGIN
//                            SELECT @LockPeriod END ELSE BEGIN select NULL END";

//                var command = new SqlCommand(sqlQuery, sqlConn);

//                DateTime.TryParse(command.ExecuteScalar().ToString(), out var lockPeriod);
//                if (lockPeriod.IsEmpty()) return null;
//                return lockPeriod;
//            }
//        }

//        public bool CheckPurchaseGrossWtRestrictionPeriod(string vehicleNum, int weight, int purchaseGrossWtRestrictionPeriod = 0)
//        {
//            if (appConfigRepository.AppConfig.TransactionOption.VehicleDeliveryRestriction == false) return true;

//            if (purchaseGrossWtRestrictionPeriod == 0) purchaseGrossWtRestrictionPeriod = appConfigRepository.AppConfig.TransactionOption.PurchaseGrossWtRestrictionPeriod;

//            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
//            {
//                var sqlQuery = $@"DECLARE @VehicleNum varchar(20); SET @VehicleNum = '{vehicleNum}'
//	                        DECLARE @grossWt int; SET @grossWt = {weight}
//	                        DECLARE @SameVehicleLockPeriod int; SET @SameVehicleLockPeriod = {purchaseGrossWtRestrictionPeriod}
//	                        DECLARE @purchaseGrossWtRestrictionPeriod Dat
//eTime; set @LockPeriod =(DATEADD(mi,-@SameVehicleLockPeriod,GETDATE()))
//	                        DECLARE @currentDate DateTime; 
//                            IF ((Select Count(*) from Inyards where VehicleNum = @VehicleNum 
//                            and GrossWt = @CommodityId and DateTimeIn > @LockPeriod ) +
//                            (Select Count(*) from PurchaseTransactions where VehicleNum = @VehicleNum 
//                            and RawMaterialId = @CommodityId and  DateTimeIn >@LockPeriod)
//	                            +
//                            (Select Count(*) from SaleTransactions where VehicleNum = @VehicleNum 
//                            and ProductId = @CommodityId and  DateTimeIn >@LockPeriod) > 0) BEGIN
//                            SELECT @LockPeriod END ELSE BEGIN select null END";

//                command.CommandText = sqlQuery;
//                var result = (int)command.ExecuteScalar();
//                return result == 0;
//            }
//        }

//        public Dictionary<string, string> ValidateInyard(Inyard model)
//        {
//            var modelStateDict = new Dictionary<string, string>();
//            if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
//            {
//                var vehicleDeliverRestriction = new VehicleDeliveryRestriction()
//                {
//                    VehicleNum = model.VehicleNum,
//                    CommodityId = model.CommodityId,
//                    DateTimeIn = model.DateTimeIn
//                };

//                var vehicleDeliveryRestrictionResult = vehicleDeliveryRestrictionRepository.CheckRestriction(vehicleDeliverRestriction);
//                if (vehicleDeliveryRestrictionResult != null) modelStateDict.Add(nameof(Inyard.VehicleNum), ValidationMessages.VehicleDeliveryInvalid(vehicleDeliveryRestrictionResult.DTRestriction));

//                var purchaseGrossRestrictionresult = new PurchaseGrossWtRestriction()
//                {
//                    VehicleNum = model.VehicleNum,
//                    Weight = model.GrossWt,
//                    DateTimeIn = model.DateTimeIn
//                };

//                var purchaseGrossRestrictionResult = purchaseGrossWtRestrictionRepository.CheckRestriction(purchaseGrossRestrictionresult);
//                if (purchaseGrossRestrictionResult != null) modelStateDict.Add(nameof(Inyard.GrossWt), ValidationMessages.PurchaseGrossInvalid(purchaseGrossRestrictionResult.DTRestriction));
//            }

//            #region VALIDATE VEHICLE NUM
//            if (model.VehicleNum.IsNull()) modelStateDict.Add(nameof(model.VehicleNum), ValidationMessages.Required("Vehicle Number"));
//            else
//            {
//                var vehicle = vehicleRepository.GetByName(model.VehicleNum);
//                if (vehicle != null) model.VehicleTypeCode = vehicle.VehicleTypeCode;
//            }
//            #endregion

//            #region VALIDATE BALE TYPE
//            if (model.BaleTypeId.IsNullOrZero()) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.Required("Bale Type"));
//            else
//            {
//                var baleType = baleTypeRepository.GetById(model.BaleTypeId);
//                if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
//                else model.BaleTypeDesc = baleType.BaleTypeDesc;
//                baleType = null;
//            }
//            #endregion

//            if (model.TransactionTypeCode == "I")
//            {
//                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
//                {
//                    if (model.GrossWt == 0) modelStateDict.Add(nameof(model.GrossWt), ValidationMessages.InvalidWeight);
//                }
//                else if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
//                {
//                    if (model.MC == 0)
//                    {
//                        modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("MC"));
//                    }
//                    if (model.TareWt == 0 || model.NetWt == 0) { modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight); }

//                    var receiptNum = refNumRepository.Get().FirstOrDefault().PurchaseReceiptNum;
//                    if (dbContext.PurchaseTransactions.AsNoTracking().Count(a => a.ReceiptNum == receiptNum) > 0)
//                        modelStateDict.Add(nameof(model.InyardNum), ValidationMessages.InvalidReceiptNum);

//                    if (model.MoistureReaderId.IsNullOrZero()) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.Required("Moisture Reader");
//                    else
//                    {
//                        model.MoistureReaderDesc = baleTypeRepository.GetById(model.BaleTypeId);
//                        if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
//                        else model.BaleTypeDesc = baleType.BaleTypeDesc;
//                        baleType = null;
//                    }
//                }

//                #region VALIDATE SUPPLIER
//                if (model.ClientId.IsNullOrZero()) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.Required("Supplier"));
//                else
//                {
//                    var supplier = supplierRepository.GetById(model.ClientId);
//                    if (supplier == null) modelStateDict.Add(nameof(model.ClientName), ValidationMessages.SupplierNotExists);
//                    else model.ClientName = supplier.SupplierName;
//                    supplier = null;
//                }
//                #endregion

//                #region VALIDATE MATERIAL
//                if (model.CommodityId.IsNullOrZero()) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.Required("Material"));
//                else
//                {
//                    var material = rawMaterialRepository.GetById(model.CommodityId);
//                    if (material == null) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.RawMaterialNotExists);
//                    else
//                    {
//                        model.CommodityDesc = material.RawMaterialDesc;
//                        model.CategoryId = material.CategoryId;
//                        model.CategoryDesc = categoryRepository.Get(new Category() { CategoryId = model.CategoryId }).Select(a => a.CategoryDesc).FirstOrDefault();
//                    }
//                    material = null;
//                }
//                #endregion

//                #region VALIDATE SOURCE
//                if (model.SourceId.IsNullOrZero()) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.Required("Source"));
//                else
//                {
//                    var source = sourceRepository.GetById(model.SourceId);
//                    if (source == null) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.SourceNotExists);
//                    else
//                    {
//                        model.SourceName = source.SourceName;
//                        model.SourceCategoryId = source.SourceCategoryId;
//                        if (model.SourceCategoryId > 0)
//                        {
//                            model.SourceCategoryDesc = sourceCategoryRepository
//                                .Get(new SourceCategory() { SourceCategoryId = model.SourceCategoryId })
//                                .Select(a => a.Description).FirstOrDefault();
//                        }
//                    }
//                    source = null;
//                }
//                #endregion

//                #region VALIDATE PO
//                var po = purchaseOrderRepository.ValidatePO(new PurchaseOrder() { PONum = model.PONum });
//                if (po == null) modelStateDict.Add(nameof(model.PONum), ValidationMessages.POInvalid);
//                else if (po.BalanceRemainingKg < -5000) modelStateDict.Add(nameof(model.PONum), ValidationMessages.PORemainingBalanceInvalid);
//                #endregion

//            }
//            if (model.TransactionTypeCode == "O")
//            {
//                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
//                {
//                    if (model.TareWt == 0) modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight);
//                }
//                else if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
//                {
//                    if (model.MC == 0)
//                    {
//                        modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("MC"));
//                    }
//                    if (model.TareWt == 0 || model.NetWt == 0) { modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight); }

//                    var receiptNum = refNumRepository.Get().FirstOrDefault().PurchaseReceiptNum;
//                    if (dbContext.SaleTransactions.AsNoTracking().Count(a => a.ReceiptNum == receiptNum) > 0)
//                        modelStateDict.Add(nameof(model.InyardNum), ValidationMessages.InvalidReceiptNum);
//                    if (model.BaleCount == 0)
//                    {
//                        modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("Bale Count"));
//                    }
//                }

//                #region VALIDATE CUSTOMER
//                if (model.ClientId.IsNullOrZero()) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.Required("Customer"));
//                else
//                {
//                    var customer = customerRepository.GetById(model.ClientId);
//                    if (customer == null) modelStateDict.Add(nameof(model.ClientName), ValidationMessages.SupplierNotExists);
//                    else model.ClientName = customer.CustomerName;
//                    customer = null;
//                }
//                #endregion

//                #region VALIDATE PRODUCT
//                var product = productRepository.Get(new Product() { ProductId = model.CommodityId }).Select(a => new { a.ProductDesc, a.CategoryId }).FirstOrDefault();
//                if (product == null) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.ProductNotExists);
//                else
//                {
//                    model.CommodityDesc = product.ProductDesc;
//                    model.CategoryId = product.CategoryId;
//                    model.CategoryDesc = categoryRepository.Get(new Category() { CategoryId = model.CategoryId }).Select(a => a.CategoryDesc).FirstOrDefault();
//                }
//                #endregion

//                #region VALIDATE HAULER
//                if (model.HaulerId.GetValueOrDefault(0) > 0) model.HaulerName = haulerRepository.GetById(model.HaulerId ?? 0).HaulerName;
//                #endregion

//                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
//                {
//                    if (model.Bales.Count() > 0)
//                    {
//                        var unRelatedBalesCount = model.Bales.Count(a => a.ProductId != model.CommodityId);
//                        if (unRelatedBalesCount > 0)
//                        {
//                            modelStateDict.Add(nameof(model.CommodityId), "Selected bales must match the product type.");
//                        };
//                    }
//                }
//            }


//            model.MoistureReaderDesc = moistureReaderRepository.GetById(model.MoistureReaderId ?? 0).Description;

//            return modelStateDict;
//        }

//        public Dictionary<string, string> ValidatePurchase(PurchaseTransaction model)
//        {
//            var modelStateDict = new Dictionary<string, string>();

//            #region VALIDATE SUPPLIER
//            if (model.SupplierId.IsNullOrZero()) modelStateDict.Add(nameof(model.SupplierId), ValidationMessages.Required("Supplier"));
//            else
//            {
//                var supplier = supplierRepository.GetById(model.SupplierId);
//                if (supplier == null) modelStateDict.Add(nameof(model.SupplierName), ValidationMessages.SupplierNotExists);
//                else model.SupplierName = supplier.SupplierName;
//                supplier = null;
//            }
//            #endregion

//            #region VALIDATE MATERIAL
//            if (model.RawMaterialId.IsNullOrZero()) modelStateDict.Add(nameof(model.RawMaterialId), ValidationMessages.Required("Material"));
//            else
//            {
//                var material = rawMaterialRepository.GetById(model.RawMaterialId);
//                if (material == null) modelStateDict.Add(nameof(model.RawMaterialId), ValidationMessages.RawMaterialNotExists);
//                else
//                {
//                    model.RawMaterialDesc = material.RawMaterialDesc;
//                    model.CategoryId = material.CategoryId;
//                    model.CategoryDesc = categoryRepository.Get(new Category() { CategoryId = model.CategoryId }).Select(a => a.CategoryDesc).FirstOrDefault();
//                }
//                material = null;
//            }
//            #endregion

//            #region VALIDATE PO
//            var po = purchaseOrderRepository.ValidatePO(new PurchaseOrder() { PONum = model.PONum });
//            if (po == null) modelStateDict.Add(nameof(model.PONum), ValidationMessages.POInvalid);
//            else if (po.BalanceRemainingKg < -5000) modelStateDict.Add(nameof(model.PONum), ValidationMessages.PORemainingBalanceInvalid);
//            #endregion

//            #region VALIDATE SOURCE
//            if (model.SourceId.IsNullOrZero()) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.Required("Source"));
//            else
//            {
//                var source = sourceRepository.GetById(model.SourceId);
//                if (source == null) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.SourceNotExists);
//                else
//                {
//                    model.SourceName = source.SourceName;
//                    model.SourceCategoryId = source.SourceCategoryId;
//                    if (model.SourceCategoryId > 0)
//                    {
//                        model.SourceCategoryDesc = sourceCategoryRepository
//                            .Get(new SourceCategory() { SourceCategoryId = model.SourceCategoryId })
//                            .Select(a => a.Description).FirstOrDefault();
//                    }
//                }
//                source = null;
//            }
//            #endregion

//            #region VALIDATE BALE TYPE
//            if (model.BaleTypeId.IsNullOrZero()) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.Required("Bale Type"));
//            else
//            {
//                var baleType = baleTypeRepository.GetById(model.BaleTypeId);
//                if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
//                else model.BaleTypeDesc = baleType.BaleTypeDesc;
//                baleType = null;
//            }
//            #endregion

//            if ((model.MoistureReaderId ?? 0) > 0)
//            {
//                var moistureReader = moistureReaderRepository.GetById(model.MoistureReaderId.Value);
//                if (moistureReader == null) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
//                else model.MoistureReaderDesc = moistureReader.Description;
//                moistureReader = null;
//            }

//            return modelStateDict;
//        }

//        public Dictionary<string, string> ValidateSale(SaleTransaction model)
//        {
//            var modelStateDict = new Dictionary<string, string>();

//            var customer = CustomerRepository.GetById(model.CustomerId);
//            if (customer == null) modelStateDict.Add(nameof(model.CustomerId), ValidationMessages.SupplierNotExists);
//            else model.CustomerName = customer.CustomerName;
//            customer = null;


//            var hauler = HaulerRepository.GetById(model.HaulerId);
//            if (hauler == null) modelStateDict.Add(nameof(model.HaulerId), ValidationMessages.RawMaterialNotExists);
//            else model.HaulerName = hauler.HaulerName;
//            hauler = null;

//            var product = ProductRepository.GetById(model.ProductId);
//            if (product == null) { modelStateDict.Add(nameof(model.ProductId), ValidationMessages.RawMaterialNotExists); }
//            else
//            {
//                model.ProductDesc = product.ProductDesc;
//                model.CategoryId = product.CategoryId ?? 0;
//            }
//            product = null;

//            var baleType = BaleTypeRepository.GetById(model.BaleTypeId);
//            if (baleType == null) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
//            else model.BaleTypeDesc = baleType.BaleTypeDesc;
//            baleType = null;

//            if ((model.MoistureReaderId ?? 0) > 0)
//            {
//                var moistureReader = moistureReaderRepository.GetById(model.MoistureReaderId.Value);
//                if (moistureReader == null) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
//                else model.MoistureReaderDesc = moistureReader.Description;
//                moistureReader = null;
//            }

//            return modelStateDict;
//        }


//    }
//}
