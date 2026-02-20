using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers;

[Authorize] 
[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductRepository productRepository, 
    IProductService productService,
    IFileStorageService fileStorageService) : ControllerBase
{
    // DTOs ligeros para mapear las peticiones
    public record ProductUpdateDto(string Name, decimal Price, int Stock, string? ImageUrl);

    // DTO para recibir imagen en base64 (para peticiones JSON desde frontend)
    public record ProductCreateJsonDto(string Name, decimal Price, int Stock, string? ImageUrl);

    // Usamos IFormFile para recibir la imagen física en la petición Multipart/Form-Data
    public record ProductCreateFormDto(string Name, decimal Price, int Stock, IFormFile? Image);

    // DTO para response con SAS URL
    public record ProductDto(Guid Id, string Name, decimal Price, int Stock, string? ImageUrl, string? ImageSasUrl);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productRepository.GetAllAsync();

        // Generar SAS URLs para cada producto que tenga imagen
        var productsWithSas = new List<ProductDto>();
        foreach (var product in products)
        {
            string? sasUrl = null;
            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                try
                {
                    sasUrl = await fileStorageService.GetImageSasUrlAsync(product.ImageUrl, expirationMinutes: 120);
                }
                catch
                {
                    // Si falla la generación de SAS, continuamos sin la URL
                }
            }

            productsWithSas.Add(new ProductDto(
                product.Id,
                product.Name,
                product.Price,
                product.Stock,
                product.ImageUrl,
                sasUrl
            ));
        }

        return Ok(productsWithSas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return NotFound(new { Message = "Producto no encontrado" });

        // Generar SAS URL para la imagen
        string? sasUrl = null;
        if (!string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            try
            {
                sasUrl = await fileStorageService.GetImageSasUrlAsync(product.ImageUrl, expirationMinutes: 120);
            }
            catch
            {
                // Si falla la generación de SAS, continuamos sin la URL
            }
        }

        var productDto = new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Stock,
            product.ImageUrl,
            sasUrl
        );

        return Ok(productDto);
    }

    /// <summary>
    /// Endpoint proxy para servir imágenes con autenticación.
    /// Alternativa a SAS URLs cuando se necesita control total del acceso.
    /// </summary>
    [HttpGet("images/{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(string fileName)
    {
        try
        {
            var imageStream = await fileStorageService.DownloadImageAsync(fileName);

            // Determinar ContentType basado en la extensión
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };

            return File(imageStream, contentType);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { Message = $"La imagen '{fileName}' no fue encontrada" });
        }
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFromJson([FromBody] ProductCreateJsonDto request)
    {
        Stream? imageStream = null;
        string? fileName = null;

        // Convertir base64 a Stream si hay imagen
        if (!string.IsNullOrWhiteSpace(request.ImageUrl) && request.ImageUrl.Contains("base64,"))
        {
            try
            {
                // Extraer el base64 (remover el prefijo data:image/jpeg;base64,)
                var base64Data = request.ImageUrl.Split(',')[1];
                var imageBytes = Convert.FromBase64String(base64Data);
                imageStream = new MemoryStream(imageBytes);

                // Detectar extensión del tipo MIME
                var mimeType = request.ImageUrl.Split(';')[0].Split(':')[1]; // image/jpeg
                var extension = mimeType.Split('/')[1]; // jpeg
                fileName = $"{Guid.NewGuid()}.{extension}";
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Formato de imagen base64 inválido", Error = ex.Message });
            }
        }

        var product = await productService.CreateProductAsync(
            request.Name,
            request.Price,
            request.Stock,
            imageStream,
            fileName
        );

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFromForm([FromForm] ProductCreateFormDto request)
    {
        var product = await productService.CreateProductAsync(
            request.Name,
            request.Price,
            request.Stock,
            request.Image?.OpenReadStream(),
            request.Image?.FileName
        );

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return NotFound(new { Message = "Producto no encontrado" });

        // Usamos el método de dominio para actualizar, no setters públicos directos
        product.UpdateDetails(request.Name, request.Price, request.Stock, request.ImageUrl);

        productRepository.Update(product);
        await productRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return NotFound(new { Message = "Producto no encontrado" });

        productRepository.Delete(product);
        await productRepository.SaveChangesAsync();

        return NoContent();
    }
}