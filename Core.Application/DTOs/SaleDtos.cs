namespace Core.Application.DTOs;

// Uso de Records en C# para DTOs inmutables
public record SaleRequestDto(List<SaleItemRequestDto> Items);

public record SaleItemRequestDto(Guid ProductId, int Quantity);