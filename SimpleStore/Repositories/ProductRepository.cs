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
                   ProductName=updateProductDto.ProductName,
                    updateProductDto.CategoryId,
                    updateProductDto.Price,
                    updateProductDto.Stock,
                    updateProductDto.IsActive
                },commandType:CommandType.StoredProcedure);
            return rows >= 0;
        }
    }
}
