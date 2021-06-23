using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using WeighingSystemUPPCV3_5_Repository.Models;
using WeighingSystemUPPCV3_5_Repository.Models.OldDbs;

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
        public DbSet<Models.PurchaseTransaction> PurchaseTransactions { get; set; }

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

        public DbSet<Models.BalingStationStatusView> BalingStationStatusViews { get; set; }
        public DbSet<Models.BusinessLicense> BusinessLicenses { get; set; }
        public DbSet<Models.MoistureSettings> MoistureSettings { get; set; }
        public DbSet<Models.Signatory> Signatories { get; set; }
        public DbSet<Models.CalibrationType> CalibrationTypes { get; set; }
        public DbSet<Models.Calibration> Calibrations { get; set; }
        public DbSet<Models.CalibrationLog> CalibrationLogs { get; set; }

        public DbSet<Models.TransactionType> TransactionTypes { get; set; }

        public DbSet<Models.MoistureReader> MoistureReaders { get; set; }
        public DbSet<Models.PurchasePriceAverageView> PurchasePriceAverageViews { get; set; }

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

        public DbSet<Models.SaleBale> SaleBales { get; set; }


        /// <summary>
        /// OLD Tables
        /// </summary>
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Tbl_PO> Tbl_POs { get; set; }
        public DbSet<BalesInv> BalesInvs { get; set; }
        public DbSet<Models.Truck> Trucks { get; set; }
        public DbSet<Beginning_Adjustment> Beginning_Adjustments { get; set; }
        public DbSet<LOOSE> LOOSEs { get; set; }
        public DbSet<InTheMachine> InTheMachines { get; set; }
        public DbSet<ABMC> ABMCs { get; set; }
        public DbSet<Models.OldDbs.Hauler> HaulersOld { get; set; }

        public DbSet<Models.OldDbs.Customer> CustomersOld { get; set; }
        public DbSet<Models.OldDbs.Supplier> SuppliersOld { get; set; }
        public DbSet<Models.OldDbs.TruckClassification> TruckClassificationsOld { get; set; }
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
            modelBuilder.Entity<PurchaseOrder>().Property(a => a.Price).HasPrecision(8, 3);
            modelBuilder.Entity<PurchaseOrderView>().Property(a => a.Price).HasPrecision(8, 3);

            //modelBuilder.Entity<Inyard>()
            //    .HasOne(a => a.PurchaseOrder)
            //    .WithMany(a=>a.Inyards)
            //    .HasPrincipalKey(a => a.PONum)
            //    .HasForeignKey(a => a.PONum);

            //modelBuilder.Entity<BalingStation>()
            //    .HasMany(a => a.BusinessLicences)
            //    .WithOne(a => a.BalingStation)
            //    .HasPrincipalKey(a => a.BalingStationNum)
            //    .HasForeignKey(a=>a.BalingStationNum);

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
