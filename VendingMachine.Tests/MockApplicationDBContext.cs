using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendingMachine.Data;
using VendingMachine.Models;

namespace VendingMachine.Tests
{
    internal class MockApplicationDBContext
    {
        public static ApplicationDBContext GetMockDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "VendingMachineTestDb")
                .Options;

            var dbContext = new ApplicationDBContext(options);

            // Seed data for testing
            dbContext.Products.AddRange(new List<Product>
        {
            new Product { ID = 1, ProductName = "Coke", Price = 1.5, Quantity = 10, CreatedDate = DateTime.Now },
            new Product { ID = 2, ProductName = "Pepsi", Price = 1.3, Quantity = 20, CreatedDate = DateTime.Now }
        });
            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
