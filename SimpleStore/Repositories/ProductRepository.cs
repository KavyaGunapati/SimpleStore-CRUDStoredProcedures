using Dapper;
using Microsoft.Data.SqlClient;
using SimpleStore.DTOs;
using System.Data;

namespace SimpleStore.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<int> CreateProductAsync(CreateProductDto createProductDto);
        Task<bool> UpdateProductAsync(int id,UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductSearchDto>> SearchProduct(string? name,int? id,decimal? minPrice,decimal? maxPrice);
        Task<PagedResult<ProductPageDto>> GetProductsPagedAsync(string? search,int? categoryId,decimal? minPrice,decimal? maxPrice,bool? onlyActive,string? sortBy,string? sortOdr,int pageNumber, int pageSize);
    }
    public class ProductRepository(IConfiguration cfg) : IProductRepository
    {
        private readonly string _connectionString = cfg.GetConnectionString("DefaultConnection");
        private SqlConnection GetConnection() => new SqlConnection(_connectionString);
        public async Task<int> CreateProductAsync(CreateProductDto createProductDto)
        {
            using var connection = GetConnection();
            var p=new DynamicParameters();
            p.Add("@Name", createProductDto.Name);
            p.Add("@CategoryId", createProductDto.CategoryId);
            p.Add("@Price", createProductDto.Price);
            p.Add("@Stock", createProductDto.Stock);
            p.Add("@NewId", dbType:DbType.Int32, direction:ParameterDirection.Output);
            await connection.ExecuteAsync(
                "dbo.usp_Product_Add", p, commandType:CommandType.StoredProcedure);
            return p.Get<int>("@NewId");
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            using var connection = GetConnection();
            var rows = await connection.ExecuteAsync(
                "dbo.usp_Products_Delete", new { ProductId = productId }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
           using var connection = GetConnection();
           return await connection.QueryAsync<ProductDto>(
                "dbo.usp_Product_GetAll", commandType:CommandType.StoredProcedure);
        }

        public async Task<ProductDto> GetProductByIdAsync(int productId)
        {
            using var connection = GetConnection();
            return await connection.QueryFirstOrDefaultAsync<ProductDto>(
                "dbo.usp_Product_GetById", new { ProductId = productId }, commandType:CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            using var connection = GetConnection();
            var rows=await connection.ExecuteAsync(
                "dbo.usp_Product_Update",new
                {
                    ProductId=id,
                   updateProductDto.ProductName,
                    updateProductDto.CategoryId,
                    updateProductDto.Price,
                    updateProductDto.Stock,
                    updateProductDto.IsActive
                },commandType:CommandType.StoredProcedure);
            return rows >= 0;
        }

        public async Task<IEnumerable<ProductSearchDto>> SearchProduct(string? name,int? id,decimal? minPrice,decimal? maxPrice)
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<ProductSearchDto>(
                "dbo.usp_Product_Search", new { ProductName = name, CategoryId = id, MinPrice = minPrice, MaxPrice = maxPrice }, commandType: CommandType.StoredProcedure);
        }

        public async Task<PagedResult<ProductPageDto>> GetProductsPagedAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? onlyActive, string? sortBy, string? sortOdr, int pageNumber, int pageSize)
        {
           using var connection= GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Search", search);
            parameters.Add("@CategoryId",categoryId);
            parameters.Add("@MinPrice", minPrice);
            parameters.Add("@MaxPrice",maxPrice);
            parameters.Add("@OnlyActive", onlyActive);
            parameters.Add("@SortBy",sortBy);
            parameters.Add("@SortOdr", sortOdr);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            using var multi = await connection.QueryMultipleAsync("dbo.usp_Product_FilterPaged", parameters, commandType: CommandType.StoredProcedure);
            var items=(await multi.ReadAsync<ProductPageDto>()).ToList();
            var total=await multi.ReadFirstAsync<int>();
            return new(items, total);
        }
    }
}
