using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Api.Controllers;

[Authorize] // Protege todo el controlador con JWT
[ApiController]
[Route("api/[controller]")]
public class SalesController(ISaleService saleService, ISaleRepository saleRepository) : ControllerBase
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

    [HttpGet("report")]
    public async Task<IActionResult> GetSalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var sales = await saleRepository.GetSalesByDateRangeAsync(startDate, endDate);

        // Paginación en memoria (para la prueba, lo ideal sería hacerlo en el repositorio con EF)
        var totalRecords = sales.Count();
        var pagedSales = sales
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.Id,
                s.Date,
                s.Total,
                ItemsCount = s.Items.Count
            });

        return Ok(new
        {
            TotalRecords = totalRecords,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
            Data = pagedSales
        });
    }
}