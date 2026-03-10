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
    public record ProductSearchDto(
    int ProductId,
    string ProductName,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName
);
public record PagedResult<T>(IReadOnlyList<T> Items, int Total);
public record ProductPageDto(int ProductId, string ProductName, decimal Price, int Stock, bool IsActive, int CategoryId, string CategoryName, DateTime CreatedAt);

}
