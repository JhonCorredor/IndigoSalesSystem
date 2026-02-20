using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Utilities.TimeZone;

namespace Presentation.Api.Controllers;

[Authorize] // Protege todo el controlador con JWT
[ApiController]
[Route("api/[controller]")]
public class SalesController(
    ISaleService saleService, 
    ISaleRepository saleRepository,
    IProductRepository productRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterSale([FromBody] SaleRequestDto request)
    {
        try
        {
            var saleId = await saleService.RegisterSaleAsync(request);
            return Created($"/api/sales/{saleId}", new { Message = "Venta registrada exitosamente", SaleId = saleId });
        }
        catch (InvalidOperationException ex) // Captura fallos de stock del Dominio
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (KeyNotFoundException ex) // Captura producto no encontrado
        {
            return NotFound(new { Error = ex.Message });
        }
    }

    [HttpGet("{saleId:guid}")]
    public async Task<IActionResult> GetSaleById(Guid saleId)
    {
        var sale = await saleRepository.GetByIdWithItemsAsync(saleId);
        if (sale is null)
            return NotFound(new { Message = "Venta no encontrada" });

        // Obtener todos los productos necesarios para esta venta
        var productIds = sale.Items.Select(i => i.ProductId).Distinct();
        var products = await productRepository.GetAllAsync();
        var productDict = products.ToDictionary(p => p.Id, p => p.Name);

        var response = new
        {
            sale.Id,
            sale.Date, // Ya está en hora de Colombia
            sale.Total,
            Items = sale.Items.Select(item => new
            {
                ProductName = productDict.TryGetValue(item.ProductId, out var name) 
                    ? name 
                    : "Producto no disponible",
                item.Quantity,
                item.UnitPrice,
                Subtotal = item.SubTotal
            })
        };

        return Ok(response);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Las fechas en BD ya están en hora de Colombia (DateTime.Now), no necesitamos convertir
        var normalizedStartDate = startDate.Date; // 00:00:00
        var normalizedEndDate = endDate.Date.AddDays(1).AddTicks(-1); // 23:59:59.9999999

        var sales = await saleRepository.GetSalesByDateRangeAsync(normalizedStartDate, normalizedEndDate);

        // Paginación
        var totalRecords = sales.Count();
        var pagedSales = sales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                s.Date, // Ya está en hora de Colombia
                s.Total,
                ItemsCount = s.Items.Count,
                s.IsActive
            });

        return Ok(new
        {
            TotalRecords = totalRecords,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
            FilterApplied = new 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                TimeZone = "America/Bogota (UTC-5)"
            },
            Data = pagedSales
        });
    }

    [HttpGet("report/all")]
    public async Task<IActionResult> GetAllSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Las fechas en BD ya están en hora de Colombia, no necesitamos convertir
        var normalizedStartDate = startDate.Date;
        var normalizedEndDate = endDate.Date.AddDays(1).AddTicks(-1);

        var sales = await saleRepository.GetSalesByDateRangeAsync(normalizedStartDate, normalizedEndDate);

        var salesData = sales.Select(s => new
        {
            s.Id,
            s.Date, // Ya está en hora de Colombia
            s.Total,
            ItemsCount = s.Items.Count,
            s.IsActive
        });

        return Ok(new
        {
            TotalRecords = sales.Count(),
            FilterApplied = new 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                TimeZone = "America/Bogota (UTC-5)"
            },
            Data = salesData
        });
    }
}