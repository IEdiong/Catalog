using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(CatalogDbContext context, ILogger logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            if (await context.Products.AnyAsync())
            {
                logger.LogInformation("Database already seeded");
                return;
            }

            logger.LogInformation("Seeding database with initial data");

            var products = new List<Product>
            {
                new("Wireless Bluetooth Headphones", 
                    "Premium quality wireless headphones with active noise cancellation and 30-hour battery life", 
                    199.99m, 50),
                    
                new("Smartphone - Latest Model", 
                    "Flagship smartphone with advanced camera system, 5G connectivity, and all-day battery", 
                    899.99m, 25),
                    
                new("Laptop Computer - Professional", 
                    "High-performance laptop with Intel Core i7, 16GB RAM, 512GB SSD, perfect for professionals", 
                    1299.99m, 15),
                    
                new("Gaming Mouse", 
                    "High-precision gaming mouse with customizable RGB lighting and programmable buttons", 
                    79.99m, 100),
                    
                new("Mechanical Keyboard", 
                    "Tactile mechanical keyboard with blue switches and customizable backlighting", 
                    149.99m, 75),
                    
                new("4K Monitor", 
                    "27-inch 4K UHD monitor with HDR support and USB-C connectivity", 
                    349.99m, 30),
                    
                new("Wireless Charging Pad", 
                    "Fast wireless charging pad compatible with all Qi-enabled devices", 
                    39.99m, 200),
                    
                new("Bluetooth Speaker", 
                    "Portable Bluetooth speaker with 360-degree sound and waterproof design", 
                    89.99m, 80),
                    
                new("Smartwatch - Fitness Edition", 
                    "Advanced fitness tracking smartwatch with heart rate monitor and GPS", 
                    299.99m, 40),
                    
                new("USB-C Hub", 
                    "Multi-port USB-C hub with HDMI, USB 3.0 ports, and power delivery", 
                    59.99m, 120)
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully with {ProductCount} products", products.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}