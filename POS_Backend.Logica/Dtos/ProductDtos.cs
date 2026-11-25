namespace POS_Backend.Logica.Dtos;

public record ProductRequest(
    string Code,
    string? Barcode,
    string Name,
    string? Description,
    string UnitOfMeasure,
    int CategoryId,
    int? SupplierId,
    decimal AverageCost,
    decimal SalePrice,
    decimal TaxRate,
    decimal Stock,
    decimal MinimumStock);

public record ProductDto(
    int Id,
    string Code,
    string? Barcode,
    string Name,
    string? Description,
    string UnitOfMeasure,
    int CategoryId,
    string CategoryName,
    int? SupplierId,
    decimal AverageCost,
    decimal SalePrice,
    decimal TaxRate,
    decimal Stock,
    decimal MinimumStock,
    bool IsActive);