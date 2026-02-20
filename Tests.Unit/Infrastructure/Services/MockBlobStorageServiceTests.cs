using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Unit.Infrastructure.Services;

/// <summary>
/// Tests for MockBlobStorageService
/// </summary>
public class MockBlobStorageServiceTests
{
    private readonly Mock<ILogger<MockBlobStorageService>> _loggerMock;
    private readonly MockBlobStorageService _service;

    public MockBlobStorageServiceTests()
    {
        _loggerMock = new Mock<ILogger<MockBlobStorageService>>();
        _service = new MockBlobStorageService(_loggerMock.Object);
    }

    [Fact]
    public async Task UploadImageAsync_WithValidFile_ReturnsValidUrl()
    {
        // Arrange
        var fileName = "test-image.jpg";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });

        // Act
        var result = await _service.UploadImageAsync(fileName, stream);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("https://indigomockstorage.blob.core.windows.net/product-images/", result);
        Assert.Contains(".jpg", result);
    }

    [Fact]
    public async Task UploadImageAsync_LogsInformation()
    {
        // Arrange
        var fileName = "test.png";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        await _service.UploadImageAsync(fileName, stream);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Iniciando subida simulada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task DeleteImageAsync_WithExistingFile_ReturnsTrue()
    {
        // Arrange
        var fileName = "existing-file.jpg";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var uploadedUrl = await _service.UploadImageAsync(fileName, stream);
        
        // Extraer el nombre único del archivo de la URL
        var uniqueFileName = uploadedUrl.Split('/').Last();

        // Act
        var result = await _service.DeleteImageAsync(uniqueFileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteImageAsync_WithNonExistingFile_ReturnsFalse()
    {
        // Arrange
        var fileName = "non-existing-file.jpg";

        // Act
        var result = await _service.DeleteImageAsync(fileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetImageUrlAsync_WithExistingFile_ReturnsUrl()
    {
        // Arrange
        var fileName = "test-get-url.jpg";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var uploadedUrl = await _service.UploadImageAsync(fileName, stream);
        var uniqueFileName = uploadedUrl.Split('/').Last();

        // Act
        var result = await _service.GetImageUrlAsync(uniqueFileName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(uploadedUrl, result);
    }

    [Fact]
    public async Task GetImageUrlAsync_WithNonExistingFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var fileName = "non-existing.jpg";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _service.GetImageUrlAsync(fileName));
    }

    [Fact]
    public async Task UploadImageAsync_MultipleTimes_GeneratesUniqueUrls()
    {
        // Arrange
        var fileName = "duplicate.jpg";
        var urls = new List<string>();

        // Act
        for (int i = 0; i < 3; i++)
        {
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var url = await _service.UploadImageAsync(fileName, stream);
            urls.Add(url);
        }

        // Assert
        Assert.Equal(3, urls.Distinct().Count());
    }

    [Fact]
    public async Task UploadImageAsync_SimulatesNetworkLatency()
    {
        // Arrange
        var fileName = "latency-test.jpg";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var startTime = DateTime.UtcNow;

        // Act
        await _service.UploadImageAsync(fileName, stream);
        var endTime = DateTime.UtcNow;

        // Assert
        var duration = (endTime - startTime).TotalMilliseconds;
        Assert.True(duration >= 500, $"Expected at least 500ms delay, but got {duration}ms");
    }
}
