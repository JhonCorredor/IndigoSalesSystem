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

    [Fact]
    public async Task RegisterSaleAsync_WithInsufficientStock_ShouldIncludeProductNameInErrorMessage()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = "Laptop Core i7";
        var product = new Product(productId, productName, 1500m, 5, null);

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        var request = new SaleRequestDto([new SaleItemRequestDto(productId, 10)]);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _saleService.RegisterSaleAsync(request));

        Assert.Contains(productName, exception.Message);
        Assert.Contains("disponible 5", exception.Message);
        Assert.Contains("solicitado 10", exception.Message);
    }

    [Fact]
    public async Task RegisterSaleAsync_WithMultipleProductsInsufficientStock_ShouldReportAllErrors()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var product1 = new Product(productId1, "Laptop", 1500m, 2, null);
        var product2 = new Product(productId2, "Mouse", 50m, 3, null);

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId1))
            .ReturnsAsync(product1);
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId2))
            .ReturnsAsync(product2);

        // Intentamos comprar más de lo disponible en ambos productos
        var request = new SaleRequestDto([
            new SaleItemRequestDto(productId1, 10),
            new SaleItemRequestDto(productId2, 5)
        ]);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _saleService.RegisterSaleAsync(request));

        // Debe reportar errores de ambos productos
        Assert.Contains("Laptop", exception.Message);
        Assert.Contains("Mouse", exception.Message);

        // No se debe guardar nada
        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterSaleAsync_WithMultipleValidProducts_ShouldReduceStockForAll()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var product1 = new Product(productId1, "Laptop", 1500m, 10, null);
        var product2 = new Product(productId2, "Mouse", 50m, 20, null);

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId1))
            .ReturnsAsync(product1);
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId2))
            .ReturnsAsync(product2);

        var request = new SaleRequestDto([
            new SaleItemRequestDto(productId1, 2),
            new SaleItemRequestDto(productId2, 5)
        ]);

        // Act
        var resultId = await _saleService.RegisterSaleAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);
        Assert.Equal(8, product1.Stock);  // 10 - 2
        Assert.Equal(15, product2.Stock); // 20 - 5

        _productRepositoryMock.Verify(repo => repo.Update(product1), Times.Once);
        _productRepositoryMock.Verify(repo => repo.Update(product2), Times.Once);
        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterSaleAsync_WhenOneProductInvalidAndOneValid_ShouldNotModifyAnyStock()
    {
        // Arrange
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var product1 = new Product(productId1, "Laptop", 1500m, 10, null); // Suficiente stock
        var product2 = new Product(productId2, "Mouse", 50m, 2, null);     // Stock insuficiente

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId1))
            .ReturnsAsync(product1);
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId2))
            .ReturnsAsync(product2);

        var request = new SaleRequestDto([
            new SaleItemRequestDto(productId1, 2),
            new SaleItemRequestDto(productId2, 5) // Pide 5, solo hay 2
        ]);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _saleService.RegisterSaleAsync(request));

        // El stock del producto válido NO debe modificarse (fail-fast)
        Assert.Equal(10, product1.Stock);
        Assert.Equal(2, product2.Stock);

        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterSaleAsync_WithExactStock_ShouldSucceedAndLeaveZeroStock()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Último Item", 100m, 5, null);

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Comprar exactamente todo el stock disponible
        var request = new SaleRequestDto([new SaleItemRequestDto(productId, 5)]);

        // Act
        var resultId = await _saleService.RegisterSaleAsync(request);

        // Assert
        Assert.NotEqual(Guid.Empty, resultId);
        Assert.Equal(0, product.Stock);
        _saleRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }
}