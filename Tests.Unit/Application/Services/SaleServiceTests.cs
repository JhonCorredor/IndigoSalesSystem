using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Services;

public class SaleServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ISaleRepository> _saleRepositoryMock;
    private readonly SaleService _saleService;

    public SaleServiceTests()
    {
        // Setup inicial para cada prueba
        _productRepositoryMock = new Mock<IProductRepository>();
        _saleRepositoryMock = new Mock<ISaleRepository>();

        _saleService = new SaleService(_productRepositoryMock.Object, _saleRepositoryMock.Object);
    }

    [Fact]
    public async Task RegisterSaleAsync_WithValidStock_ShouldCreateSaleAndReduceStock()
    {
        // Arrange (Preparar)
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Laptop Core i7", 1500m, 10, null);

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Usamos Collection Expressions de C# 12: []
        var request = new SaleRequestDto([new SaleItemRequestDto(productId, 2)]);

        // Act (Actuar)
        var resultId = await _saleService.RegisterSaleAsync(request);

        // Assert (Afirmar)
        Assert.NotEqual(Guid.Empty, resultId); // Se generó un ID de venta
        Assert.Equal(8, product.Stock); // El dominio descontó el stock correctamente (10 - 2)

        // Verificamos que se llamó a la base de datos para guardar la transacción
        _saleRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Sale>()), Times.Once);
        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterSaleAsync_WhenProductDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Simulamos que el repositorio no encuentra el producto (retorna null)
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var request = new SaleRequestDto([new SaleItemRequestDto(productId, 2)]);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _saleService.RegisterSaleAsync(request));

        // Verificamos que si falla, NUNCA se llama al SaveChangesAsync (protección de datos)
        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterSaleAsync_WithInsufficientStock_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        // Producto con solo 1 unidad en stock
        var product = new Product(productId, "Laptop Core i7", 1500m, 1, null);

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Intentamos comprar 2 unidades
        var request = new SaleRequestDto([new SaleItemRequestDto(productId, 2)]);

        // Act & Assert
        // Debe lanzar la excepción controlada por la entidad de Dominio
        await Assert.ThrowsAsync<InvalidOperationException>(() => _saleService.RegisterSaleAsync(request));

        // Verificamos que la transacción se abortó
        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}