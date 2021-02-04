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
using WeighingSystemUPPCV3_5_Repository.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository.Repositories
{
    public class TransValidationRepository : ITransValidationRepository
    {
        private readonly DatabaseContext dbContext;
        private readonly IBalingStationRepository balingStationRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IAppConfigRepository appConfigRepository;
        private readonly IMoistureReaderRepository moistureReaderRepository;
        private readonly IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository;
        private readonly IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository;
        private readonly IPurchaseOrderRepository purchaseOrderRepository;
        private readonly IReferenceNumberRepository refNumRepository;
        private readonly IVehicleRepository vehicleRepository;
        private readonly IUserAccountRepository userAccountRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;
        private readonly IHaulerRepository haulerRepository;
        private readonly IBaleTypeRepository baleTypeRepository;
        private readonly ISourceRepository sourceRepository;
        private readonly ISourceCategoryRepository sourceCategoryRepository;
        private readonly ISubSupplierRepository subSupplierRepository;
        private readonly IRawMaterialRepository rawMaterialRepository;

        public TransValidationRepository(DatabaseContext dbContext,
            IBalingStationRepository balingStationRepository,
            ICustomerRepository customerRepository,
            ISupplierRepository supplierRepository,
            IRawMaterialRepository rawMaterialRepository,
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IHaulerRepository haulerRepository,
            IBaleTypeRepository baleTypeRepository,
            ISourceRepository sourceRepository,
            ISourceCategoryRepository sourceCategoryRepository,
            ISubSupplierRepository subSupplierRepository,
            IAppConfigRepository appConfigRepository,
            IMoistureReaderRepository moistureReaderRepository,
            IVehicleDeliveryRestrictionRepository vehicleDeliveryRestrictionRepository,
            IPurchaseGrossWtRestrictionRepository purchaseGrossWtRestrictionRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IReferenceNumberRepository refNumRepository,
            IPurchaseTransactionRepository purchaseTransactionRepository,
            ISaleTransactionRepository saleTransactionRepository,
            IVehicleRepository vehicleRepository,
            IUserAccountRepository userAccountRepository)
        {
            this.dbContext = dbContext;
            this.balingStationRepository = balingStationRepository;
            this.customerRepository = customerRepository;
            this.supplierRepository = supplierRepository;
            this.appConfigRepository = appConfigRepository;
            this.moistureReaderRepository = moistureReaderRepository;
            this.vehicleDeliveryRestrictionRepository = vehicleDeliveryRestrictionRepository;
            this.purchaseGrossWtRestrictionRepository = purchaseGrossWtRestrictionRepository;
            this.purchaseOrderRepository = purchaseOrderRepository;
            this.refNumRepository = refNumRepository;
            this.vehicleRepository = vehicleRepository;
            this.userAccountRepository = userAccountRepository;
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
            this.haulerRepository = haulerRepository;
            this.baleTypeRepository = baleTypeRepository;
            this.sourceRepository = sourceRepository;
            this.sourceCategoryRepository = sourceCategoryRepository;
            this.subSupplierRepository = subSupplierRepository;
            this.rawMaterialRepository = rawMaterialRepository;
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
            return customerRepository.Get().Count(a => a.CustomerId == id) > 0;
        }
        public bool SupplierExists(long id)
        {
            return supplierRepository.Get().Count(a => a.SupplierId == id) > 0;
        }
        public bool HaulerExists(long id)
        {
            return haulerRepository.Get().Count(a => a.HaulerId == id) > 0;
        }
        public bool RawMaterialExists(long id)
        {
            return rawMaterialRepository.Get().Count(a => a.RawMaterialId == id) > 0;
        }
        public bool ProductExists(long id)
        {
            return productRepository.Get().Count(a => a.ProductId == id) > 0;
        }
        public bool BaleTypeExists(long id)
        {
            return baleTypeRepository.Get().Count(a => a.BaleTypeId == id) > 0;
        }
        public bool MoistureReaderExists(long id)
        {
            return moistureReaderRepository.Get().Count(a => a.MoistureReaderId == id) > 0;
        }
        public bool SourceExists(long id)
        {
            return sourceRepository.Get().Count(a => a.SourceId == id) > 0;
        }
        public bool UserAccountExists(string id)
        {
            return userAccountRepository.Get().Count(a => a.UserAccountId == id) > 0;
        }

        public Dictionary<string, string> ValidateInyardWeighing(Inyard model)
        {
            var modelStateDict = new Dictionary<string, string>();

            if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN ||
                model.TransactionProcess == SysUtility.Enums.TransactionProcess.UPDATE_WEIGH_IN)
            {
                if (model.IsOfflineIn == false)
                {
                    if (model.VehicleNum != model.VehicleNumOld || model.VehicleNumOld == null)
                    {
                        var vehicleDeliverRestriction = new VehicleDeliveryRestriction(model.VehicleNum, model.CommodityId) { DateTimeIn = model.DateTimeIn };
                        var vehicleDeliveryRestrictionResult = vehicleDeliveryRestrictionRepository.CheckRestriction(vehicleDeliverRestriction);
                        if (vehicleDeliveryRestrictionResult != null) modelStateDict.Add(nameof(Inyard.VehicleNum), ValidationMessages.VehicleDeliveryInvalid(vehicleDeliveryRestrictionResult.DTRestriction));

                        var purchaseGrossRestrictionresult = new PurchaseGrossWtRestriction(model.VehicleNum, model.CommodityId) { DateTimeIn = model.DateTimeIn };
                        var purchaseGrossRestrictionResult = purchaseGrossWtRestrictionRepository.CheckRestriction(purchaseGrossRestrictionresult);
                        if (purchaseGrossRestrictionResult != null) modelStateDict.Add(nameof(Inyard.GrossWt), ValidationMessages.PurchaseGrossInvalid(purchaseGrossRestrictionResult.DTRestriction));
                    }
                }

                #region VALIDATE INSPECTOR/WEIGHER
                if (model.WeigherInId.IsNull()) modelStateDict.Add(nameof(model.WeigherInId), ValidationMessages.Required("Inspector is required."));
                else if (UserAccountExists(model.WeigherInId) == false) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.UserNotExists);
                #endregion

                if (modelStateDict.Count > 0) return modelStateDict;
            } else 
            {
                #region VALIDATE INSPECTOR/WEIGHER
                if (model.WeigherOutId.IsNull()) modelStateDict.Add(nameof(model.WeigherOutId), ValidationMessages.Required("Inspector is required."));
                else if (UserAccountExists(model.WeigherOutId) == false) modelStateDict.Add(nameof(model.WeigherOutId), ValidationMessages.UserNotExists);
                #endregion
            }

            #region VALIDATE VEHICLE NUM
            if (model.VehicleNum.IsNull()) modelStateDict.Add(nameof(model.VehicleNum), ValidationMessages.Required("Vehicle Number"));
            #endregion

            #region VALIDATE BALE TYPE
            if (model.BaleTypeId.IsNullOrZero()) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.Required("Bale Type"));
            else if (BaleTypeExists(model.BaleTypeId) == false) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
            #endregion

            if (model.TransactionTypeCode == "I")
            {
                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_IN)
                {
                    if (model.GrossWt == 0) modelStateDict.Add(nameof(model.GrossWt), ValidationMessages.InvalidWeight);
                }
                else if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
                {
                    if (model.MC == 0) modelStateDict.Add(nameof(model.MC), ValidationMessages.Required("MC"));
                    if (model.TareWt == 0 || model.NetWt == 0) { modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight); }

                    #region VALIDATE RECEIPT NUM
                    //var receiptNum = refNumRepository.Get().FirstOrDefault().PurchaseReceiptNum;
                    if (dbContext.PurchaseTransactions.AsNoTracking().Count(a => a.ReceiptNum == model.InyardNum) > 0)
                        modelStateDict.Add(nameof(model.InyardNum), ValidationMessages.InvalidReceiptNum);
                    #endregion

                    #region VALIDATE MOISTURE READER
                    if (model.MoistureReaderId.IsNullOrZero()) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.Required("Moisture Reader"));
                    else if (MoistureReaderExists(model.MoistureReaderId ?? 0) == false) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
                    #endregion
                }

                #region VALIDATE DR
                if (model.DRNum.IsNull()) modelStateDict.Add(nameof(model.DRNum), ValidationMessages.Required("DR Number"));
                #endregion
                #region VALIDATE SUPPLIER
                if (model.ClientId.IsNullOrZero()) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.Required("Supplier"));
                else if (SupplierExists(model.ClientId) == false) modelStateDict.Add(nameof(model.ClientName), ValidationMessages.SupplierNotExists);
                #endregion

                #region VALIDATE MATERIAL
                if (model.CommodityId.IsNullOrZero()) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.Required("Material"));
                else if (RawMaterialExists(model.CommodityId) == false) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.RawMaterialNotExists);
                #endregion

                #region VALIDATE SOURCE
                if (model.SourceId.IsNullOrZero()) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.Required("Source"));
                else if (SourceExists(model.SourceId) == false) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.SourceNotExists);
                #endregion

                #region VALIDATE PO
                var po = purchaseOrderRepository.ValidatePO(new PurchaseOrder() { PONum = model.PONum });
                if (po == null) modelStateDict.Add(nameof(model.PONum), ValidationMessages.POInvalid);
                else if (po.BalanceRemainingKg < -5000) modelStateDict.Add(nameof(model.PONum), ValidationMessages.PORemainingBalanceInvalid);
                #endregion
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
                    if (model.TareWt == 0 || model.NetWt == 0) { modelStateDict.Add(nameof(model.TareWt), ValidationMessages.InvalidWeight); }

                    var receiptNum = refNumRepository.Get().FirstOrDefault().PurchaseReceiptNum;
                    if (dbContext.SaleTransactions.AsNoTracking().Count(a => a.ReceiptNum == receiptNum) > 0)
                        modelStateDict.Add(nameof(model.InyardNum), ValidationMessages.InvalidReceiptNum);
                    if (model.BaleCount == 0)
                    {
                        modelStateDict.Add(nameof(model.BaleCount), ValidationMessages.Required("Bale Count"));
                    }

                    #region VALIDATE MOISTURE READER
                    if (model.MoistureReaderId.IsNullOrZero()) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.Required("Moisture Reader"));
                    else if (MoistureReaderExists(model.MoistureReaderId ?? 0) == false) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
                    #endregion
                }

                #region VALIDATE CUSTOMER / HAULER
                if (model.ClientId.IsNullOrZero() && model.HaulerId.IsNullOrZero()) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.Required("Customer/Hauler"));
                if (model.ClientId.IsNullOrZero() == false) if (CustomerExists(model.ClientId) == false) modelStateDict.Add(nameof(model.ClientId), ValidationMessages.CustomerNotExists);
                if (model.HaulerId.IsNullOrZero() == false) if (HaulerExists(model.HaulerId??0) == false) modelStateDict.Add(nameof(model.HaulerId), ValidationMessages.HaulerNotExists);
                #endregion

                #region VALIDATE PRODUCT
                if (model.CommodityId.IsNullOrZero()) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.Required("Product"));
                else if (ProductExists(model.CommodityId) ==false) modelStateDict.Add(nameof(model.CommodityId), ValidationMessages.ProductNotExists);
                #endregion

      

                #region VALIDATE BALES
                if (model.TransactionProcess == SysUtility.Enums.TransactionProcess.WEIGH_OUT)
                {
                    if (model.Bales.Count() > 0)
                    {
                        var unRelatedBalesCount = model.Bales.Count(a => a.ProductId != model.CommodityId);
                        if (unRelatedBalesCount > 0)
                        {
                            modelStateDict.Add(nameof(model.CommodityId), "Selected bales must match the product type.");
                        };
                    }
                }
                #endregion
            }

            return modelStateDict;
        }
   
        public Dictionary<string, string> ValidatePurchase(PurchaseTransaction model)
        {
            var modelStateDict = new Dictionary<string, string>();

            #region VALIDATE VEHICLE NUM
            if (model.VehicleNum.IsNull()) modelStateDict.Add(nameof(model.VehicleNum), ValidationMessages.Required("Vehicle Number"));
            #endregion

            #region VALIDATE BALE TYPE
            if (model.BaleTypeId.IsNullOrZero()) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.Required("Bale Type"));
            else if (BaleTypeExists(model.BaleTypeId) == false) modelStateDict.Add(nameof(model.BaleTypeId), ValidationMessages.BaleTypeNotExists);
            #endregion

            #region VALIDATE SUPPLIER
            if (model.SupplierId.IsNullOrZero()) modelStateDict.Add(nameof(model.SupplierId), ValidationMessages.Required("Supplier"));
            else if (SupplierExists(model.SupplierId)==false) modelStateDict.Add(nameof(model.SupplierId), ValidationMessages.SupplierNotExists);
            #endregion

            #region VALIDATE MATERIAL
            if (model.RawMaterialId.IsNullOrZero()) modelStateDict.Add(nameof(model.RawMaterialId), ValidationMessages.Required("Material"));
            else if (RawMaterialExists(model.RawMaterialId)==false) modelStateDict.Add(nameof(model.RawMaterialId), ValidationMessages.RawMaterialNotExists);
            #endregion

            #region VALIDATE SOURCE
            if (model.SourceId.IsNullOrZero()) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.Required("Source"));
            else if (SourceExists(model.SourceId)==false) modelStateDict.Add(nameof(model.SourceId), ValidationMessages.SourceNotExists);
            #endregion

            #region VALIDATE PO
            var po = purchaseOrderRepository.ValidatePO(new PurchaseOrder() { PONum = model.PONum });
            if (po == null) modelStateDict.Add(nameof(model.PONum), ValidationMessages.POInvalid);
            else if (po.BalanceRemainingKg < -5000) modelStateDict.Add(nameof(model.PONum), ValidationMessages.PORemainingBalanceInvalid);
            #endregion

            #region VALIDATE MOISTURE READER
            if (model.MoistureReaderId.IsNullOrZero()) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.Required("Moisture Reader"));
            else if (MoistureReaderExists(model.MoistureReaderId ?? 0)==false) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
            #endregion

            #region VALIDATE INSPECTOR/WEIGHER
            if (model.WeigherOutId.IsNull()) modelStateDict.Add(nameof(model.WeigherOutId), ValidationMessages.Required("Inspector is required."));
            else if (UserAccountExists(model.WeigherOutId) == false) modelStateDict.Add(nameof(model.WeigherOutId), ValidationMessages.UserNotExists);
            #endregion

            return modelStateDict;
        }

        public Dictionary<string, string> ValidateSale(SaleTransaction model)
        {
            var modelStateDict = new Dictionary<string, string>();

            #region VALIDATE CUSTOMER / HAULER
            if (model.CustomerId.IsNullOrZero() && model.HaulerId.IsNullOrZero()) modelStateDict.Add(nameof(model.CustomerId), ValidationMessages.Required("Customer/Hauler"));
            if (model.CustomerId.IsNullOrZero() == false) if (CustomerExists(model.CustomerId) == false) modelStateDict.Add(nameof(model.CustomerId), ValidationMessages.CustomerNotExists);
            if (model.HaulerId.IsNullOrZero() == false) if (HaulerExists(model.HaulerId) ==false) modelStateDict.Add(nameof(model.HaulerId), ValidationMessages.HaulerNotExists);
            #endregion

            #region VALIDATE PRODUCT
            if (model.ProductId.IsNullOrZero()) modelStateDict.Add(nameof(model.ProductId), ValidationMessages.Required("Product"));
            else if (ProductExists(model.ProductId) ==false) modelStateDict.Add(nameof(model.ProductId), ValidationMessages.ProductNotExists);

            #endregion

            #region VALIDATE BALES

            if (model.SaleBales?.Count() > 0)
            {
                var unRelatedBalesCount = model.SaleBales.Select(a => a.Bale).Count(a => a.ProductId != model.ProductId);
                if (unRelatedBalesCount > 0)
                {
                    modelStateDict.Add(nameof(model.ProductId), "Selected bales must match the product type.");
                };
            }

            #endregion

            #region VALIDATE MOISTURE READER
            if (model.MoistureReaderId.IsNullOrZero()) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.Required("Moisture Reader"));
            else if (MoistureReaderExists(model.MoistureReaderId ?? 0) ==false) modelStateDict.Add(nameof(model.MoistureReaderId), ValidationMessages.MoistureReaderNotExists);
            #endregion

            #region VALIDATE INSPECTOR/WEIGHER
            if (model.WeigherOutId.IsNull()) modelStateDict.Add(nameof(model.WeigherOutId), ValidationMessages.Required("Inspector is required."));
            else if (UserAccountExists(model.WeigherOutId) == false) modelStateDict.Add(nameof(model.WeigherOutId), ValidationMessages.UserNotExists);
            #endregion

            return modelStateDict;
        }

    
    }
}
