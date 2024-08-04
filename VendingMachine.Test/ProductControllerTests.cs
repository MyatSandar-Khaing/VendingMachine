using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using VendingMachine.Controllers;
using VendingMachine.Data;
using VendingMachine.Models;
using Xunit;

namespace VendingMachine.Tests
{
    public class ProductControllerTests
    {
        private readonly ProductController _controller;
        private readonly ApplicationDBContext _context;

        public ProductControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDBContext(options);
            _controller = new ProductController(_context);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfProducts()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            // Act
            var result = await _controller.Index(sortOrder: null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProductsIndexViewModel>(viewResult.ViewData.Model);
            Assert.NotEmpty(model.Products);
        }

        [Fact]
        public async Task Details_ReturnsAViewResult_WithAProduct()
        {
            // Arrange
            var product = new Product { ID = 1, ProductName = "Test Product", Price = 10, Quantity = 5, CreatedDate = DateTime.Now };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Product>(viewResult.ViewData.Model);
            Assert.Equal(1, model.ID);
        }

        // Additional tests for Create, Edit, Delete, etc.
    }
}
