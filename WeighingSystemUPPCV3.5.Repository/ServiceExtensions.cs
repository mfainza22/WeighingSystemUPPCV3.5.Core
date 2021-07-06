using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeighingSystemUPPCV3_5_Repository.IRepositories;
using WeighingSystemUPPCV3_5_Repository.Repositories;
using SysUtility.Config;
using SysUtility.Config.Interfaces;

namespace WeighingSystemUPPCV3_5_Repository
{
    public static class ServiceExtensions
    {
        public static void AddDbContextService(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging());

        }

        public static void AddRepositoryService(this IServiceCollection services)
        {
            services.AddSingleton<IAppConfigRepository, AppConfigRepository>();
            services.AddTransient<IVehicleDeliveryRestrictionRepository, VehicleDeliveryRestrictionRepository>();
            services.AddTransient<IPurchaseGrossWtRestrictionRepository, PurchaseGrossWtRestrictionRepository>();
            services.AddTransient<IReminderRepository, ReminderRepository>();
            services.AddTransient<IMoistureReaderRepository, MoistureReaderRepository>();
            services.AddTransient<IBalingStationRepository, BalingStationRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ISupplierRepository, SupplierRepository>();
            services.AddTransient<IHaulerRepository, HaulerRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IRawMaterialRepository, RawMaterialRepository>();
            services.AddTransient<IBaleTypeRepository, BaleTypeRepository>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IVehicleTypeRepository, VehicleTypeRepository>();
            services.AddTransient<ISourceRepository, SourceRepository>();
            services.AddTransient<ISourceCategoryRepository, SourceCategoryRepository>();
            services.AddTransient<ISubSupplierRepository, SubSupplierRepository>();
            services.AddTransient<IUserAccountRepository, UserAccountRepository>();
            services.AddTransient<IBusinessLicenseRepository, BusinessLicenseRepository>();
            services.AddTransient<IReferenceNumberRepository, ReferenceNumberRepository>();
            services.AddTransient<IMoistureSettingsRepository, MoistureSettingsRepository>();
            services.AddTransient<ISignatoryRepository, SignatoryRepository>();
            services.AddTransient<ICalibrationTypeRepository, CalibrationTypeRepository>();
            services.AddTransient<ICalibrationRepository, CalibrationRepository>();
            services.AddTransient<IInyardRepository, InyardRepository>();
            services.AddTransient<ITransValidationRepository, TransValidationRepository>();
            services.AddTransient<ITransactionTypeRepository, TransactionTypeRepository>();
            services.AddTransient<IBaleRepository, BaleRepository>();
            services.AddTransient<IPaperMillRepository, PaperMillRepository>();
            services.AddTransient<IPurchaseTransactionRepository, PurchaseTransactionRepository>();
            services.AddTransient<ISaleTransactionRepository, SaleTransactionRepository>();
            services.AddTransient<IReturnedVehicleRepository, ReturnedVehicleRepository>();
            services.AddTransient<IBeginningInvAdjRepository, BeginningInvAdjRepository>();
            services.AddTransient<IActualBalesMCRepository, ActualBalesMCRepository>();
            services.AddTransient<ILooseBaleRepository, LooseBaleRepository>();
            services.AddTransient<IMachineUnBaledWasteRepository, MachineUnBaledWasteRepository>();
            services.AddTransient<ISourceCategoryTargetRepository, SourceCategoryTargetRepository>();
            services.AddTransient<IPrintLogRepository, PrintLogRepository>();
            services.AddTransient<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddTransient<IReportingRepository, ReportingRepository>();
            services.AddTransient<IAuditLogRepository, AuditLogRepository>();
            services.AddTransient<IAuditLogEventRepository, AuditLogEventRepository>();
        }
    }
}
