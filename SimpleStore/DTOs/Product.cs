namespace SimpleStore.DTOs
{

    public record ProductDto(
        int ProductId,
        string Name,
        decimal Price,
        int Stock,
        bool IsActive,
        int CategoryId,
        string CategoryName,
        DateTime CreatedAt
    );

    public record CreateProductDto(
        string Name,
        int CategoryId,
        decimal Price,
        int Stock
    );

    public record UpdateProductDto(
        string ProductName,
        int CategoryId,
        decimal Price,
        int Stock,
        bool IsActive
    );

}
