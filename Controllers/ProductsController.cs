using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Models;
using Microsoft.AspNetCore.Authorization;
using ProductManagementApi.Attributes;

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "DualAuth")] // Allow both JWT and Basic authentication
    public class ProductsController : ControllerBase
    {

        private readonly Services.IProductService _productService;

        public ProductsController(Services.IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [RequireUserOrAdmin] // Both User and Admin can view products
        public async Task<ActionResult<object>> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (products, totalCount) = await _productService.GetProductsAsync(pageNumber, pageSize);
            return Ok(new {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }


        [HttpGet("{id}")]
        [RequireUserOrAdmin] // Both User and Admin can view individual products
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return product;
        }

        [HttpPost]
        [RequireAdmin] // Only Admin can create products
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var createdProduct = await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        [RequireAdmin] // Only Admin can update products
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            product.Id = id;
            var updatedProduct = await _productService.UpdateProductAsync(product);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [RequireAdmin] // Only Admin can delete products
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
