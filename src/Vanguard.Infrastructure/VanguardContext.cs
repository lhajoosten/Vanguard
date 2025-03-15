using Microsoft.EntityFrameworkCore;

namespace Vanguard.Infrastructure
{
    public class VanguardContext : DbContext
    {
        public VanguardContext(DbContextOptions<VanguardContext> options) : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }
        //public DbSet<VPIdentity> VPIdentities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<User>().ToTable("Users");
            //modelBuilder.Entity<VPIdentity>().ToTable("VPIdentities");
        }
    }
}
