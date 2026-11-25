using Microsoft.Extensions.Logging;
using POS_Backend.Logica.Dtos;
using POS_Backend.Logica.Exceptions;
using POS_Backend.Logica.Services;
using static POS_Backend.Logica.Exceptions.DomainExceptionFilter;

namespace POS_Backend.Logica.Domain;

public class ProductDomain
{
    private const int CodeMaxLength = 50;
    private const int BarcodeMaxLength = 50;
    private const int NameMaxLength = 75;
    private const int DescriptionMaxLength = 500;
    private const int UnitOfMeasureMaxLength = 20;
    private const decimal TaxRateMin = 0;
    private const decimal TaxRateMax = 100;

    private readonly IProductService _productService;
    private readonly ILogger<ProductDomain> _logger;

    public ProductDomain(IProductService productService, ILogger<ProductDomain> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<List<ProductDto>> GetAllAsync(bool onlyActive = true, CancellationToken ct = default)
    {
        try
        {
            var products = await _productService.GetAllAsync(onlyActive, ct);
            return products.Select(ToDto).ToList();
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al listar los productos.");
            throw new DomainException("No se pudo obtener la lista de productos.", ex);
        }
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var product = await GetExistingAsync(id, ct);
            return ToDto(product);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al obtener el producto {ProductId}.", id);
            throw new DomainException($"No se pudo obtener el producto con id {id}.", ex);
        }
    }

    public async Task<List<ProductDto>> SearchAsync(string? text, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<ProductDto>();

            var products = await _productService.SearchActiveAsync(text.Trim(), ct);
            return products.Select(ToDto).ToList();
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al buscar productos.");
            throw new DomainException("No se pudo realizar la búsqueda de productos.", ex);
        }
    }

    public async Task<List<ProductDto>> GetLowStockAsync(CancellationToken ct = default)
    {
        try
        {
            var products = await _productService.GetLowStockAsync(ct);
            return products.Select(ToDto).ToList();
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al consultar los productos con stock bajo.");
            throw new DomainException("No se pudo obtener la lista de productos con stock bajo.", ex);
        }
    }

    public async Task<ProductDto> CreateAsync(ProductRequest request, CancellationToken ct = default)
    {
        try
        {
            var data = Normalize(request);
            await ValidateAsync(data, null, ct);

            var product = new Product
            {
                Code = data.Code,
                Barcode = data.Barcode,
                Name = data.Name,
                Description = data.Description,
                UnitOfMeasure = data.UnitOfMeasure,
                CategoryId = data.CategoryId,
                SupplierId = data.SupplierId,
                AverageCost = data.AverageCost,
                SalePrice = data.SalePrice,
                TaxRate = data.TaxRate,
                Stock = data.Stock,
                MinimumStock = data.MinimumStock,
                IsActive = true
            };

            var created = await _productService.AddAsync(product, ct);
            return ToDto(created);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al crear el producto.");
            throw new DomainException("No se pudo crear el producto.", ex);
        }
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductRequest request, CancellationToken ct = default)
    {
        try
        {
            var product = await GetExistingAsync(id, ct);

            var data = Normalize(request);
            await ValidateAsync(data, id, ct);

            product.Code = data.Code;
            product.Barcode = data.Barcode;
            product.Name = data.Name;
            product.Description = data.Description;
            product.UnitOfMeasure = data.UnitOfMeasure;
            product.CategoryId = data.CategoryId;
            product.SupplierId = data.SupplierId;
            product.AverageCost = data.AverageCost;
            product.SalePrice = data.SalePrice;
            product.TaxRate = data.TaxRate;
            product.MinimumStock = data.MinimumStock;

            var updated = await _productService.UpdateAsync(product, ct);
            return ToDto(updated);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            _logger.LogError(ex, "Error al actualizar el producto {ProductId}.", id);
            throw new DomainException($"No se pudo actualizar el producto con id {id}.", ex);
        }
    }

    public Task DeactivateAsync(int id, CancellationToken ct = default) => ChangeStatusAsync(id, false, ct);

    public Task ActivateAsync(int id, CancellationToken ct = default) => ChangeStatusAsync(id, true, ct);

    private async Task ChangeStatusAsync(int id, bool active, CancellationToken ct)
    {
        try
        {
            var product = await GetExistingAsync(id, ct);

            if (product.IsActive == active) return;

            product.IsActive = active;
            await _productService.UpdateAsync(product, ct);
        }
        catch (Exception ex) when (IsUnexpected(ex))
        {
            var action = active ? "activar" : "desactivar";
            _logger.LogError(ex, "Error al {Action} el producto {ProductId}.", action, id);
            throw new DomainException($"No se pudo {action} el producto con id {id}.", ex);
        }
    }

    private async Task<Product> GetExistingAsync(int id, CancellationToken ct) =>
        await _productService.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"No existe el producto con id {id}.");

    private async Task ValidateAsync(ProductRequest data, int? currentId, CancellationToken ct)
    {
        ValidateFormat(data);
        await ValidateReferencesAsync(data, currentId, ct);
    }

    private static void ValidateFormat(ProductRequest data)
    {
        var errors = new List<string>();

        if (data.Code.Length == 0) errors.Add("El código es obligatorio.");
        else if (data.Code.Length > CodeMaxLength) errors.Add($"El código no puede pasar de {CodeMaxLength} caracteres.");

        if (data.Barcode is { Length: > BarcodeMaxLength })
            errors.Add($"El código de barras no puede pasar de {BarcodeMaxLength} caracteres.");

        if (data.Name.Length == 0) errors.Add("El nombre es obligatorio.");
        else if (data.Name.Length > NameMaxLength) errors.Add($"El nombre no puede pasar de {NameMaxLength} caracteres.");

        if (data.Description is { Length: > DescriptionMaxLength })
            errors.Add($"La descripción no puede pasar de {DescriptionMaxLength} caracteres.");

        if (data.UnitOfMeasure.Length == 0) errors.Add("La unidad de medida es obligatoria.");
        else if (data.UnitOfMeasure.Length > UnitOfMeasureMaxLength)
            errors.Add($"La unidad de medida no puede pasar de {UnitOfMeasureMaxLength} caracteres.");

        if (data.SalePrice <= 0) errors.Add("El precio de venta debe ser mayor a 0.");
        if (data.AverageCost < 0) errors.Add("El costo no puede ser negativo.");

        if (data.TaxRate < TaxRateMin || data.TaxRate > TaxRateMax)
            errors.Add($"El impuesto debe estar entre {TaxRateMin} y {TaxRateMax}.");

        if (data.Stock < 0) errors.Add("El stock no puede ser negativo.");
        if (data.MinimumStock < 0) errors.Add("El stock mínimo no puede ser negativo.");

        if (errors.Count > 0) throw new BusinessRuleException(errors);
    }

    private async Task ValidateReferencesAsync(ProductRequest data, int? currentId, CancellationToken ct)
    {
        var errors = new List<string>();

        if (await _productService.ExistsByCodeAsync(data.Code, currentId, ct))
            errors.Add("Ya existe un producto con ese código.");

        if (data.Barcode is not null && await _productService.ExistsByBarcodeAsync(data.Barcode, currentId, ct))
            errors.Add("Ya existe un producto con ese código de barras.");

        if (!await _productService.CategoryExistsAsync(data.CategoryId, ct))
            errors.Add("La categoría no existe.");

        if (data.SupplierId is not null && !await _productService.SupplierExistsAsync(data.SupplierId.Value, ct))
            errors.Add("El proveedor no existe.");

        if (errors.Count > 0) throw new BusinessRuleException(errors);
    }

    private static ProductRequest Normalize(ProductRequest request) => request with
    {
        Code = (request.Code ?? string.Empty).Trim(),
        Barcode = CleanText(request.Barcode),
        Name = (request.Name ?? string.Empty).Trim(),
        Description = CleanText(request.Description),
        UnitOfMeasure = (request.UnitOfMeasure ?? string.Empty).Trim()
    };

    private static string? CleanText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Code, p.Barcode, p.Name, p.Description, p.UnitOfMeasure,
        p.CategoryId, p.Category?.Name ?? string.Empty, p.SupplierId,
        p.AverageCost, p.SalePrice, p.TaxRate, p.Stock, p.MinimumStock, p.IsActive);
}