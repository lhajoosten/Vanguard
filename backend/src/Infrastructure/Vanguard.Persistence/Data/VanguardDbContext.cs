using Microsoft.EntityFrameworkCore;

namespace Vanguard.Persistence.Data
{
    public class VanguardDbContext : DbContext
    {
        public VanguardDbContext(DbContextOptions<VanguardDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure your entities here
        }
    }
}
