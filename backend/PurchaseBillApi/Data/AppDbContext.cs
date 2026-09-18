using Microsoft.EntityFrameworkCore;
using PurchaseBillApi.Models;

namespace PurchaseBillApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<LocationDetail> LocationDetails => Set<LocationDetail>();
    public DbSet<PurchaseBillItem> PurchaseBillItems => Set<PurchaseBillItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocationDetail>()
            .HasIndex(l => new { l.Location_Code, l.Username })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
