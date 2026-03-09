using Dapper;
using Microsoft.Data.SqlClient;
using SimpleStore.DTOs;
using System.Data;
namespace SimpleStore.Repositories;

public interface IOrderRepository
{
    // Orders
    Task<(OrderHeaderDto Header, List<OrderItemViewDto> Items)> CreateAsync(PlaceOrderRequest request);
    Task<(OrderHeaderDto Header, List<OrderItemViewDto> Items)?> GetByIdAsync(int orderId);
    Task<(List<OrderHeaderDto> Items, int Total)> ListPagedAsync(int pageNumber, int pageSize);
    Task<(OrderHeaderDto Header, List<OrderItemViewDto> Items)> UpdateReplaceItemsAsync(int orderId, UpdateOrderRequest request);
    Task<bool> DeleteAsync(int orderId);

  
}

public class OrderRepository(IConfiguration cfg) : IOrderRepository
{
    private readonly string _conn = cfg.GetConnectionString("DefaultConnection")!;
    private SqlConnection GetConn() => new(_conn);

    private static DataTable BuildTvp(IEnumerable<OrderItemInput> items)
    {
        var dt = new DataTable();
        dt.Columns.Add("ProductId", typeof(int));
        dt.Columns.Add("Quantity", typeof(int));
        foreach (var it in items) dt.Rows.Add(it.ProductId, it.Quantity);
        return dt;
    }

    // Orders
    public async Task<(OrderHeaderDto Header, List<OrderItemViewDto> Items)> CreateAsync(PlaceOrderRequest request)
    {
        using var conn = GetConn();
        var p = new DynamicParameters();
        p.Add("@CustomerId", request.CustomerId);
        p.Add("@Items", BuildTvp(request.Items).AsTableValuedParameter("dbo.OrderItemsTVP"));
        p.Add("@NewOrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        using var multi = await conn.QueryMultipleAsync("dbo.usp_Order_Place", p, commandType: CommandType.StoredProcedure);
        var header = await multi.ReadFirstAsync<OrderHeaderDto>();
        var items = (await multi.ReadAsync<OrderItemViewDto>()).ToList();
        return (header, items);
    }

    public async Task<(OrderHeaderDto Header, List<OrderItemViewDto> Items)?> GetByIdAsync(int orderId)
    {
        using var conn = GetConn();
        using var multi = await conn.QueryMultipleAsync("dbo.use_Order_GetById", new { OrderId = orderId }, commandType: CommandType.StoredProcedure);
        var header = await multi.ReadFirstOrDefaultAsync<OrderHeaderDto>();
        if (header is null) return null;
        var items = (await multi.ReadAsync<OrderItemViewDto>()).ToList();
        return (header, items);
    }

    public async Task<(List<OrderHeaderDto> Items, int Total)> ListPagedAsync(int pageNumber, int pageSize)
    {
        using var conn = GetConn();
        using var multi = await conn.QueryMultipleAsync("dbo.use_Order_ListPaged", new { PageNumber = pageNumber, PageSize = pageSize }, commandType: CommandType.StoredProcedure);
        var items = (await multi.ReadAsync<OrderHeaderDto>()).ToList();
        var total = await multi.ReadFirstAsync<int>();
        return (items, total);
    }

    public async Task<(OrderHeaderDto Header, List<OrderItemViewDto> Items)> UpdateReplaceItemsAsync(int orderId, UpdateOrderRequest request)
    {
        using var conn = GetConn();
        var p = new DynamicParameters();
        p.Add("@OrderId", orderId);
        p.Add("@CustomerId", request.CustomerId);
        p.Add("@Items", BuildTvp(request.Items).AsTableValuedParameter("dbo.OrderItemsTVP"));

        using var multi = await conn.QueryMultipleAsync("dbo.use_Order_Update", p, commandType: CommandType.StoredProcedure);
        var header = await multi.ReadFirstAsync<OrderHeaderDto>();
        var items = (await multi.ReadAsync<OrderItemViewDto>()).ToList();
        return (header, items);
    }

    public async Task<bool> DeleteAsync(int orderId)
    {
        using var conn = GetConn();
            await conn.ExecuteAsync("dbo.usp_Order_Delete", new { OrderId = orderId }, commandType: CommandType.StoredProcedure);
            return true;
    }
}