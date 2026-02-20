using Core.Application.DTOs;

namespace Core.Application.Interfaces;

public interface ISaleService
{
    Task<Guid> RegisterSaleAsync(SaleRequestDto request);
}