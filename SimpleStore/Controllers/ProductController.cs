using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleStore.Repositories;

namespace SimpleStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;
        
        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllProductsAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(DTOs.CreateProductDto createProductDto)
        {
            var newId = await _repository.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProductById), new { id = newId }, null);
        }
         [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, DTOs.UpdateProductDto updateProductDto)
        {
                var success = await _repository.UpdateProductAsync(id, updateProductDto);
                if (!success) return NotFound();
                return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _repository.DeleteProductAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchProduct(string? name,int? id,decimal? minPrice,decimal? maxPrice)
        {
            var product = await _repository.SearchProduct(name,id,minPrice,maxPrice);
            if (product == null) return NotFound();
            return Ok(product);
        }
        [HttpGet("filter-paged")]
        public async Task<IActionResult> FilterAndGetPagedProducts(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? onlyActive, string? sortBy, string? sortOdr, int pageNumber, int pageSize)
        {
            var result=await _repository.GetProductsPagedAsync(search,categoryId,minPrice,maxPrice,onlyActive,sortBy,sortOdr,pageNumber,pageSize);
            return Ok(result);
        }
        }
}
