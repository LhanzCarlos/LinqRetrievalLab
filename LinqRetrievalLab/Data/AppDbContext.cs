using LinqRetrievalLab.Models;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Electronics"
                },
                new Category
                {
                    Id = 2,
                    Name = "Food"
                },
                new Category
                {
                    Id = 3,
                    Name = "School Supplies"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Wireless Mouse",
                    Price = 599.00m,
                    Stock = 15,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Keyboard",
                    Price = 899.00m,
                    Stock = 10,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "USB Cable",
                    Price = 249.00m,
                    Stock = 25,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 4,
                    Name = "Burger",
                    Price = 120.00m,
                    Stock = 20,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 5,
                    Name = "French Fries",
                    Price = 80.00m,
                    Stock = 30,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 6,
                    Name = "Notebook",
                    Price = 75.00m,
                    Stock = 40,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 7,
                    Name = "Ballpen",
                    Price = 25.00m,
                    Stock = 50,
                    CategoryId = 3
                }
            );
        }
    }
}