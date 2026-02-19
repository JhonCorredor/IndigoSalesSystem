using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración fluida de Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        // Configuración fluida de Sale
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Total).HasColumnType("decimal(18,2)");

            // Configurar la relación 1 a muchos entre Venta y sus Ítems
            entity.HasMany(s => s.Items)
                  .WithOne()
                  .HasForeignKey("SaleId") // Shadow property para mantener el dominio limpio
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración fluida de SaleItem
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(si => si.Id);
            entity.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");

            // Relación con el Producto
            entity.HasOne(si => si.Product)
                  .WithMany()
                  .HasForeignKey(si => si.ProductId)
                  .OnDelete(DeleteBehavior.Restrict); // No borrar productos si tienen ventas asociadas
        });
    }
}