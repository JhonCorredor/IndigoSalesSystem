using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers;

[cite_start]
[Authorize] // Protege todo el CRUD de productos 
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository productRepository) : ControllerBase
{
    // DTOs ligeros (Records de C# 9+) para mapear las peticiones
    public record ProductCreateDto(string Name, decimal Price, int Stock, string? ImageUrl);
    public record ProductUpdateDto(string Name, decimal Price, string? ImageUrl);

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
    public async Task<IActionResult> Create([FromBody] ProductCreateDto request)
    {
        // El Dominio (Product) se encarga de inicializar su estado válido
        var product = new Product(Guid.NewGuid(), request.Name, request.Price, request.Stock, request.ImageUrl);

        await productRepository.AddAsync(product);
        await productRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateDto request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null) return NotFound(new { Message = "Producto no encontrado" });

        // Usamos el método de dominio para actualizar, no setters públicos directos
        product.UpdateDetails(request.Name, request.Price, request.ImageUrl);

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