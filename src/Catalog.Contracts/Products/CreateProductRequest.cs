namespace Catalog.Contracts.Products;

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);

public record CreateProductResponse(Guid Id);

public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price);

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    bool IsActive);