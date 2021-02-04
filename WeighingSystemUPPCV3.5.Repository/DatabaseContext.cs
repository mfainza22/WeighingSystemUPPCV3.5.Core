using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeighingSystemUPPCV3_5_Repository.Models;

namespace WeighingSystemUPPCV3_5_Repository
{
    public class DatabaseContext : DbContext
    {
        public static readonly ILoggerFactory logger
      = LoggerFactory.Create(builder =>
      {
          builder
              .AddFilter((category, level) =>
                  category == DbLoggerCategory.Database.Command.Name
                  && level == LogLevel.Information)
              .AddConsole();
      });

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Models.Customer> Customers { get; set; }
        public DbSet<Models.Supplier> Suppliers { get; set; }

        public DbSet<Models.Hauler> Haulers { get; set; }
        public virtual DbSet<Models.Category> Categories { get; set; }

        public virtual DbSet<Models.RawMaterial> RawMaterials { get; set; }
        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.BaleType> BaleTypes { get; set; }
        public DbSet<Models.Bale> Bales { get; set; }

        public DbSet<Models.Vehicle> Vehicles { get; set; }
        public DbSet<Models.VehicleType> VehicleTypes { get; set; }
        public DbSet<Models.SubSupplier> SubSuppliers { get; set; }
        public DbSet<Models.Source> Sources { get; set; }
        public DbSet<Models.SourceCategory> SourceCategories { get; set; }
        public DbSet<Models.ReferenceNumber> ReferenceNumbers { get; set; }
        public DbSet<Models.UserAccount> UserAccounts { get; set; }

        public DbSet<Models.UserAccountPermission> UserAccountPermissions { get; set; }
        public DbSet<Models.BalingStation> BalingStations { get; set; }
        public DbSet<Models.BusinessLicense> BusinessLicenses { get; set; }
        public DbSet<Models.MoistureSettings> MoistureSettings { get; set; }
        public DbSet<Models.Signatory> Signatories { get; set; }
        public DbSet<Models.CalibrationType> CalibrationTypes { get; set; }
        public DbSet<Models.Calibration> Calibrations { get; set; }
        public DbSet<Models.CalibrationLog> CalibrationLogs { get; set; }

        public DbSet<Models.TransactionType> TransactionTypes { get; set; }

        public DbSet<Models.MoistureReader> MoistureReaders { get; set; }
        public DbSet<Models.PurchaseTransaction> PurchaseTransactions { get; set; }

        public DbSet<Models.MoistureReaderLogPivotView> MoistureReaderLogPivotViews { get; set; }

        public DbSet<Models.SaleTransaction> SaleTransactions { get; set; }
        public DbSet<Models.MoistureReaderLog> moistureReaderLogs { get; set; }
        public DbSet<Models.Inyard> Inyards { get; set; }
        public DbSet<Models.ReturnedVehicle> ReturnedVehicles { get; set; }


        public DbSet<Models.ActualBalesMC> ActualBalesMCs { get; set; }
        public DbSet<Models.LooseBale> LooseBales { get; set; }
        public DbSet<Models.BeginningInvAdj> BeginningInvAdjs { get; set; }
        public DbSet<Models.BeginningInvAdjView> BeginningInvAdjViews { get; set; }
        public DbSet<Models.MachineUnBaledWaste> MachineUnBaledWastes { get; set; }

        public DbSet<Models.SourceCategoryTarget> SourceCategoryTargets { get; set; }
        public DbSet<Models.PrintLog> PrintLogs { get; set; }
        public DbSet<Models.Reminder> Reminders { get; set; }
        public DbSet<Models.PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Models.PurchaseOrderView> PurchaseOrderViews { get; set; }
        public DbSet<Models.VehicleDeliveryRestriction> VehicleDeliveryRestrictions { get; set; }
        public DbSet<Models.PurchaseGrossWtRestriction> PurchaseGrossWtRestrictions { get; set; }

        public DbSet<Models.PurchaseSalesMCComparisonView> PurchaseSaleMCComparisonViews { get; set; }

        public DbSet<Models.ReportDay> ReportDays { get; set; }
        public DbSet<Models.TVFInventory> TVFInventories { get; set; }
        public DbSet<Models.AuditLog> AuditLogs { get; set; }
        public DbSet<Models.AuditLogEvent> AuditLogEvents { get; set; }
        /// <summary>
        /// OLD Tables
        /// </summary>
        public DbSet<Models.Purchase> Purchases { get; set; }
        public DbSet<Models.Sale> Sales { get; set; }
        public DbSet<Models.SaleBale> SaleBales{ get; set; }

        public DbSet<Models.Tbl_PO> Tbl_POs { get; set; }
        public DbSet<Models.BalesInv> BalesInvs { get; set; }
        public DbSet<Models.Truck> Trucks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(logger);
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<RawMaterial>().Property(a => a.Price).HasPrecision(8, 3);
            modelBuilder.Entity<PurchaseTransaction>().Property(a => a.Price).HasPrecision(8, 3);

      

            //modelBuilder.Entity<SaleTransaction>(entity =>
            //{
            //    entity.HasIndex(a => a.ReceiptNum).IsUnique(true);
            //});

            //        modelBuilder.Entity<SaleTransaction>()
            //.HasOne(e => e.ReturnedVehicle)
            //.WithOne(a=>a.SaleTransaction)
            //.OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder.Entity<Inyard>(entity =>
            //{


            //});
            //modelBuilder.Entity<Category>(entity =>
            //{
            //    entity.HasKey("CategoryId");
            //});

            //modelBuilder.Entity<RawMaterial>(entity =>
            //{
            //    entity.HasKey("RawMaterialId");

            //    entity.HasOne(pt => pt.Category)
            //       .WithMany(p => p.RawMaterials)
            //       .HasForeignKey(pt => pt.CategoryId)
            //       .HasConstraintName("FK_Categories_RawMaterials");
            //});
        }

    }
}
