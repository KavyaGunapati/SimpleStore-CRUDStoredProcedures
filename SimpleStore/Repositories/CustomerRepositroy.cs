using Dapper;
using Microsoft.Data.SqlClient;
using SimpleStore.DTOs;

namespace SimpleStore.Repositories
{
    public interface ICustomerRepositroy
    {
        Task<IEnumerable<Customer>> GetAll();
        Task<Customer> GetById(int id);
        Task<int> CreateCustomer(Customer customer);
        Task<bool> UpdateCustomer(Customer customer);
        Task<bool> DeleteCustomer(int id);

    }
    public class CustomerRepositroy(IConfiguration config) : ICustomerRepositroy
    {
        private readonly string _connectionString = config.GetConnectionString("DefaultConnection");
        private SqlConnection GetConnection()=>new SqlConnection(_connectionString);

        
        public async Task<int> CreateCustomer(Customer customer)
        {
            using var connection = GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerName", customer.CustomerName);
            parameters.Add("@NewId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
            await connection.ExecuteScalarAsync<int>(
                "dbo.use_Customer_Create", parameters, commandType: System.Data.CommandType.StoredProcedure);
            return parameters.Get<int>("@NewId");
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            using var connection = GetConnection();
            var rows = await connection.ExecuteAsync("dbo.use_Customer_Delete", new { CustomerId = id }, commandType: System.Data.CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            using var connection = GetConnection();
            return await connection.QueryAsync<Customer>("dbo.use_Customer_GetAll", commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Customer> GetById(int id)
        {
            using var connection = GetConnection();
            return await connection.QuerySingleOrDefaultAsync<Customer>("dbo.use_Customer_GetById", new { CustomerId = id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            using var connection = GetConnection();
            var rows= await connection.ExecuteAsync("dbo.use_Customer_Update", new {  customer.CustomerId, customer.CustomerName }, commandType: System.Data.CommandType.StoredProcedure);
            return rows > 0;
        }
    }
}
