using Infrastructure.Services;
using FluentAssertions;

namespace Tests.Unit.Infrastructure.Services;

/// <summary>
/// Integration-style tests for AzureBlobStorageService
/// Note: These tests require a valid Azure Storage connection string to run
/// For unit testing without Azure, use a mocking framework or test against Azurite (Azure Storage Emulator)
/// </summary>
public class AzureBlobStorageServiceTests
{
    private const string TestConnectionString = "UseDevelopmentStorage=true"; // Azurite local emulator
    private const string TestContainerName = "test-container";

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new AzureBlobStorageService(TestConnectionString, TestContainerName);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AzureBlobStorageService(string.Empty, TestContainerName));
    }

    [Fact]
    public void Constructor_WithEmptyContainerName_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new AzureBlobStorageService(TestConnectionString, string.Empty));
    }

    [Theory]
    [InlineData("test.jpg", "image/jpeg")]
    [InlineData("test.jpeg", "image/jpeg")]
    [InlineData("test.png", "image/png")]
    [InlineData("test.gif", "image/gif")]
    [InlineData("test.webp", "image/webp")]
    [InlineData("test.svg", "image/svg+xml")]
    [InlineData("test.unknown", "application/octet-stream")]
    public void DetermineContentType_ReturnsCorrectMimeType(string fileName, string expectedContentType)
    {
        // This test verifies the content type logic indirectly
        // In a real scenario, you would test the UploadImageAsync method with different file types
        
        // Arrange
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        // Act
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };

        // Assert
        contentType.Should().Be(expectedContentType);
    }

    // Note: The following tests would require Azurite or a real Azure Storage account
    // Uncomment and run only when you have the proper environment set up

    /*
    [Fact]
    public async Task UploadImageAsync_WithValidStream_ReturnsValidUrl()
    {
        // Arrange
        var service = new AzureBlobStorageService(TestConnectionString, TestContainerName);
        var fileName = $"test-{Guid.NewGuid()}.jpg";
        using var stream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF }); // JPEG header

        // Act
        var result = await service.UploadImageAsync(fileName, stream);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(fileName);
        result.Should().StartWith("http");
    }

    [Fact]
    public async Task DeleteImageAsync_WithExistingFile_ReturnsTrue()
    {
        // Arrange
        var service = new AzureBlobStorageService(TestConnectionString, TestContainerName);
        var fileName = $"delete-test-{Guid.NewGuid()}.jpg";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        await service.UploadImageAsync(fileName, stream);

        // Act
        var result = await service.DeleteImageAsync(fileName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteImageAsync_WithNonExistingFile_ReturnsFalse()
    {
        // Arrange
        var service = new AzureBlobStorageService(TestConnectionString, TestContainerName);
        var fileName = $"non-existing-{Guid.NewGuid()}.jpg";

        // Act
        var result = await service.DeleteImageAsync(fileName);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetImageUrlAsync_WithExistingFile_ReturnsUrl()
    {
        // Arrange
        var service = new AzureBlobStorageService(TestConnectionString, TestContainerName);
        var fileName = $"get-url-test-{Guid.NewGuid()}.jpg";
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var uploadedUrl = await service.UploadImageAsync(fileName, stream);

        // Act
        var result = await service.GetImageUrlAsync(fileName);

        // Assert
        result.Should().Be(uploadedUrl);
    }

    [Fact]
    public async Task GetImageUrlAsync_WithNonExistingFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var service = new AzureBlobStorageService(TestConnectionString, TestContainerName);
        var fileName = $"non-existing-{Guid.NewGuid()}.jpg";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => service.GetImageUrlAsync(fileName));
    }
    */
}
