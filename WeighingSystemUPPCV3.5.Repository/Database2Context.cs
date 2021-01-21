using Microsoft.EntityFrameworkCore;

namespace WeighingSystemUPPCV3_5_Repository
{
    public class Database2Context : DbContext
    {
        public Database2Context(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Models.Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

    }
}
