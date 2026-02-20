using Core.Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Shared.Utilities.TimeZone;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar todas las propiedades DateTime para que se almacenen en UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    // Las fechas se almacenan en UTC en la base de datos
                    property.SetColumnType("datetime2");
                }
            }
        }

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        // Sale configuration
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Total).HasColumnType("decimal(18,2)");

            entity.HasMany(s => s.Items)
                  .WithOne()
                  .HasForeignKey("SaleId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(s => s.Items)
                  .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // SaleItem configuration
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(si => si.Id);
            entity.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(si => si.Product)
                  .WithMany()
                  .HasForeignKey(si => si.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
            entity.Property(r => r.Description).HasMaxLength(200);
            entity.HasIndex(r => r.Name).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(200);
            entity.Property(u => u.FirstName).HasMaxLength(50);
            entity.Property(u => u.LastName).HasMaxLength(50);

            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Apply seed data configurations
        modelBuilder.ApplyConfiguration(new ProductSeedConfiguration());
        modelBuilder.ApplyConfiguration(new SaleSeedConfiguration());
        modelBuilder.ApplyConfiguration(new SaleItemSeedConfiguration());
        modelBuilder.ApplyConfiguration(new RoleSeedConfiguration());
        modelBuilder.ApplyConfiguration(new UserSeedConfiguration());
    }
}