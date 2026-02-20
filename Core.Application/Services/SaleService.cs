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
            .Handle<Exception>() 
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        return await retryPolicy.ExecuteAsync(async () =>
        {
            // Validación anticipada: verificar stock de todos los productos antes de procesar
            var stockErrors = new List<string>();
            var productsToUpdate = new List<(Product Product, int Quantity)>();

            foreach (var item in request.Items)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new KeyNotFoundException($"El producto con ID {item.ProductId} no existe.");

                if (product.Stock < item.Quantity)
                {
                    stockErrors.Add($"Stock insuficiente para '{product.Name}': disponible {product.Stock}, solicitado {item.Quantity}.");
                }
                else
                {
                    productsToUpdate.Add((product, item.Quantity));
                }
            }

            // Si hay errores de stock, lanzar excepción con todos los detalles
            if (stockErrors.Count > 0)
            {
                throw new InvalidOperationException(string.Join(" ", stockErrors));
            }

            // Procesar la venta solo si todos los productos tienen stock suficiente
            // NOTA: Usar DateTime.Now para mantener consistencia con datos existentes en la BD
            // (las ventas anteriores se guardaron con hora local de Colombia)
            var sale = new Sale(Guid.NewGuid(), DateTime.Now);

            foreach (var (product, quantity) in productsToUpdate)
            {
                product.RemoveStock(quantity);
                productRepository.Update(product);
                sale.AddItem(product.Id, quantity, product.Price);
            }

            await saleRepository.AddAsync(sale);

            // Guarda la venta, sus ítems y la actualización del stock en una sola transacción
            await saleRepository.SaveChangesAsync();

            return sale.Id;
        });
    }
}