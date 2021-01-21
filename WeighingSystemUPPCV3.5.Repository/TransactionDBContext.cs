using Microsoft.EntityFrameworkCore;

namespace WeighingSystemUPPCV3_5_Repository
{
    public class TransactionDBContext : DbContext
    {
        public TransactionDBContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Models.Inyard> Inyards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
