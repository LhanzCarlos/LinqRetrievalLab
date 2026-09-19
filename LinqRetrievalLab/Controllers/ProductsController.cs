using LinqRetrievalLab.Data;
using LinqRetrievalLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return product;
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == product.CategoryId);

            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID does not match.");
            }

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound("Product not found.");
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == product.CategoryId);

            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/products/search?name=mouse
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
            [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name is required.");
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(name))
                .ToListAsync();

            return products;
        }

        // GET: api/products/price-range?lower=100&upper=500
        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPriceRange(
            [FromQuery] decimal lower,
            [FromQuery] decimal upper)
        {
            if (lower > upper)
            {
                return BadRequest("Lower price cannot be greater than upper price.");
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Price >= lower && p.Price <= upper)
                .ToListAsync();

            return products;
        }

        // GET: api/products/stock-sum
        [HttpGet("stock-sum")]
        public async Task<ActionResult<int>> GetStockSum()
        {
            var totalStock = await _context.Products
                .Select(p => p.Stock)
                .DefaultIfEmpty(0)
                .SumAsync();

            return totalStock;
        }

        // GET: api/products/average-price
        [HttpGet("average-price")]
        public async Task<ActionResult<decimal>> GetAveragePrice()
        {
            var averagePrice = await _context.Products
                .Select(p => p.Price)
                .DefaultIfEmpty(0)
                .AverageAsync();

            return averagePrice;
        }

        // GET: api/products/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetProductCount()
        {
            var count = await _context.Products.CountAsync();

            return count;
        }
    }
}