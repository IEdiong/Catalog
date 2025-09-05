namespace Catalog.Contracts.Orders;

public record CreateOrderRequest(
    string CustomerName,
    string CustomerEmail,
    List<OrderItemRequestDto> Items);

public record OrderItemRequestDto(
    Guid ProductId,
    int Quantity);

public record CreateOrderResponse(Guid Id);

public record OrderResponse(
    Guid Id,
    string CustomerName,
    string CustomerEmail,
    string Status,
    decimal TotalAmount,
    DateTime OrderDate,
    DateTime? CompletedDate,
    List<OrderItemResponse> Items);

public record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    decimal LineTotal);