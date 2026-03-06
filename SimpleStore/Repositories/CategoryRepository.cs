using Dapper;
using Microsoft.Data.SqlClient;
using SimpleStore.DTOs;
using System.Data;

namespace SimpleStore.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
        Task<int> CreateCategoryAsync(CategoryDto createCategoryDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
    public class CategoryRepository(IConfiguration config) : ICategoryRepository
    {
        private readonly string _connectionString = config.GetConnectionString("DefaultConnection");
        private SqlConnection GetConnection() => new SqlConnection(_connectionString);
        public async Task<int> CreateCategoryAsync(CategoryDto createCategoryDto)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryName",createCategoryDto.CategoryName);
            parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            await connection.ExecuteScalarAsync<int>(
                "dbo.usp_Categories_Add", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int>("@NewId");
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            using var connection = GetConnection();
            var rows = await connection.ExecuteAsync(
                "dbo.usp_Categories_Delete", new { CategoryId = categoryId }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<CategoryDto>(
                "dbo.usp_Category_GetAll", commandType: CommandType.StoredProcedure);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int categoryId)
        {
            using var connection = GetConnection();
            return await connection.QuerySingleOrDefaultAsync<CategoryDto>(
                "dbo.usp_Categories_GetById", new { CategoryId = categoryId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryDto updateCategoryDto)
        {
            using var connection = GetConnection();
            var rows = await connection.ExecuteAsync(
                "dbo.usp_Categories_Update", new { CategoryId = id, updateCategoryDto.CategoryName }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }
    }
}
