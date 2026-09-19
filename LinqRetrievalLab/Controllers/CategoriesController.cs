using LinqRetrievalLab.Data;
using LinqRetrievalLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/categories/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            return category;
        }

        // GET: api/categories/1/products
        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int id)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == id);

            if (!categoryExists)
            {
                return NotFound("Category not found.");
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return products;
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.Id },
                category);
        }

        // PUT: api/categories/1
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("Category ID does not match.");
            }

            var existingCategory = await _context.Categories
                .FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category name is required.");
            }

            existingCategory.Name = category.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/categories/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            if (category.Products.Any())
            {
                return Conflict(
                    "Cannot delete category because it still has products.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}