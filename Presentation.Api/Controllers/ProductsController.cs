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
    IProductService productService) : ControllerBase
{
    // DTOs ligeros para mapear las peticiones
    public record ProductUpdateDto(string Name, decimal Price, int Stock, string? ImageUrl);

    // DTO para recibir imagen en base64 (para peticiones JSON desde frontend)
    public record ProductCreateJsonDto(string Name, decimal Price, int Stock, string? ImageUrl);

    // Usamos IFormFile para recibir la imagen física en la petición Multipart/Form-Data
    public record ProductCreateFormDto(string Name, decimal Price, int Stock, IFormFile? Image);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productRepository.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return NotFound(new { Message = "Producto no encontrado" });
        return Ok(product);
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