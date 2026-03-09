using Microsoft.AspNetCore.Mvc;
using SimpleStore.Repositories;

namespace SimpleStore.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepositroy _repository;
        public CustomerController(ICustomerRepositroy repository)
        {
            _repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] DTOs.Customer customer)
        {
            var newId = await _repository.CreateCustomer(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = newId }, null);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _repository.GetById(id);
            return result != null ? Ok(result) : NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _repository.GetAll();
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerById(int id)
        {
            var result = await _repository.DeleteCustomer(id);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] DTOs.Customer customer)
        {
            var result = await _repository.UpdateCustomer(customer);
            return Ok(result);
        }
    }
}
