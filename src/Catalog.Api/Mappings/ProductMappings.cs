using Catalog.Contracts.Products;
using Catalog.Domain.Products;

namespace Catalog.Api.Mappings;

public static class ProductMappings
{
    public static ProductResponse ToProductResponse(this Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.IsActive);
    }

    public static IEnumerable<ProductResponse> ToProductResponses(this IEnumerable<Product> products)
    {
        return products.Select(ToProductResponse);
    }
}