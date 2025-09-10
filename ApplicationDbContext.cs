using Dashboard.Models;
using Microsoft.EntityFrameworkCore;


namespace Dashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<Dispenser> Dispensers { get; set; }
        public DbSet<DispenserLog> DispenserLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dispenser>()
                .HasKey(d => d.DispenserID);

            modelBuilder.Entity<DispenserLog>()
                .HasOne(dl => dl.Dispenser)
                .WithMany()
                .HasForeignKey(dl => dl.DispenserID);

            modelBuilder.Entity<Dispenser>()
                .HasOne(d => d.Unit)
                .WithMany()
                .HasForeignKey(d => d.UnitID);
        }
    }
}
