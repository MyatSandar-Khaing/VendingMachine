using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using VendingMachine.Controllers;
using VendingMachine.Data;
using VendingMachine.Dto;
using VendingMachine.Models;
using VendingMachine.Tests;

[TestClass]
public class ProductsControllerTests
{
    private ProductController _controller;
    private ApplicationDBContext _context;

    [TestInitialize]
    public void Setup()
    {
        // Arrange
        _context = MockApplicationDBContext.GetMockDbContext();
        _controller = new ProductController(_context);
    }

    [TestMethod]
    public async Task Index_Returns_ViewResult_With_ListOfProducts()
    {
        // Act
        var result = await _controller.Index(null, 1, 10);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ProductsIndexViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Products.Count);
    }

    [TestMethod]
    public async Task Details_Returns_NotFound_When_Id_Is_Null()
    {
        // Act
        var result = await _controller.Details(null);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task Details_Returns_ViewResult_With_Product()
    {
        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as Product;
        Assert.IsNotNull(model);
        Assert.AreEqual("Coke", model.ProductName);
    }

   
}
