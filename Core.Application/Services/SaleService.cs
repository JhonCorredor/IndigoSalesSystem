using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Polly;
using Polly.Retry;

namespace Core.Application.Services;

public class SaleService(IProductRepository productRepository, ISaleRepository saleRepository) : ISaleService
{
    public async Task<Guid> RegisterSaleAsync(SaleRequestDto request)
    {
        // 1. Definir Patrón de Resiliencia (Retry con Exponential Backoff)
        // Reintenta hasta 3 veces si ocurre un error transitorio, esperando 2, 4 y 8 segundos.
        var retryPolicy = Policy
            .Handle<Exception>() // En un entorno real, filtraríamos por DbUpdateException o TimeoutException
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // 2. Ejecutar la lógica de negocio protegida por Polly
        return await retryPolicy.ExecuteAsync(async () =>
        {
            var sale = new Sale(Guid.NewGuid(), DateTime.UtcNow);

            foreach (var item in request.Items)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new KeyNotFoundException($"El producto con ID {item.ProductId} no existe.");

                // El Dominio valida si hay stock suficiente. Si no, lanza excepción y aborta.
                product.RemoveStock(item.Quantity);
                productRepository.Update(product);

                sale.AddItem(product.Id, item.Quantity, product.Price);
            }

            await saleRepository.AddAsync(sale);

            // Guarda la venta, sus ítems y la actualización del stock en una sola transacción
            await saleRepository.SaveChangesAsync();

            return sale.Id;
        });
    }
}