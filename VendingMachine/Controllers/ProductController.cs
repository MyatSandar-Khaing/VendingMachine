using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VendingMachine.Data;
using VendingMachine.Dto;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDBContext _context;
        public ProductController(ApplicationDBContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index(string sortOrder, int pageNumber = 1, int pageSize = 10)
        {

            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortOrder) ? "Name" : sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var products = from p in _context.Products
                           select p;

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "Name":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "Price":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(p => p.CreatedDate);
                    break;
                default:
                    products = products.OrderBy(p => p.CreatedDate);
                    break;
            }

            var totalCount = await products.CountAsync();
            var pagedProducts = await products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new ProductsIndexViewModel
            {
                Products = pagedProducts,
                CurrentSort = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(model);

        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Price,Quantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProductName,Price,Quantity")] Product product)
        {
            if (id != product.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedDate = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Purchase(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.Quantity < quantity)
            {
                return BadRequest("Product not available or insufficient quantity.");
            }

            int userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                userId = int.Parse(userIdClaim.Value);
            }
            else
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            var transaction = new PurchaseTransaction
            {
                ProductID = id,
                Quantity = quantity,
                TotalPrice = product.Price * quantity,
                CustomerID = userId,
                TransactionDate = DateTime.UtcNow
            };

            product.Quantity -= quantity;

            _context.PurchaseTransactions.Add(transaction);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> PurchaseList(string sortOrder, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["CurrentSort"] = string.IsNullOrEmpty(sortOrder) ? "Name" : sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var products = from p in _context.PurchaseTransactions
                           select p;

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Product.ProductName);
                    break;
                case "Name":
                    products = products.OrderBy(p => p.Product.ProductName);
                    break;
                case "Quantity":
                    products = products.OrderBy(p => p.Quantity);
                    break;
                case "quantity_desc":
                    products = products.OrderByDescending(p => p.Quantity);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(p => p.TransactionDate);
                    break;
                default:
                    products = products.OrderBy(p => p.TransactionID);
                    break;
            }

            int userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                userId = int.Parse(userIdClaim.Value);
            }
            else
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            var totalCount = await products.Where(x => x.CustomerID == userId).CountAsync();
            var transactions = await products
                .Include(t => t.Product)
                .Where(x => x.CustomerID == userId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PurchaseIndexViewModel
            {
                PurchaseTransactions = transactions,
                CurrentSort = sortOrder,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return View(model);
        }
    }
}
