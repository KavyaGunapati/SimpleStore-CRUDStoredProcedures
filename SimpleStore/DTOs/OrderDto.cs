namespace SimpleStore.DTOs
{

    // Requests
    public record OrderItemInput(int ProductId, int Quantity);
    public record PlaceOrderRequest(int CustomerId, List<OrderItemInput> Items);
    public record UpdateOrderRequest(int CustomerId, List<OrderItemInput> Items);
    public record AddOrderItemRequest(int ProductId, int Quantity);
    public record UpdateOrderItemQtyRequest(int Quantity);

    // Results (match SP SELECT column order exactly)
    public record OrderHeaderDto(
        int OrderId,
        int CustomerId,
        DateTime OrderDate,
        decimal Total
    );

    public record OrderItemViewDto(
        int OrderItemId,
        int ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal
    );
}
