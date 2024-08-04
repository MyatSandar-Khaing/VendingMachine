using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VendingMachine.Data;
using VendingMachine.Dto;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ProductAPIController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product is null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.ID }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost("EditProduct/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Product product)
        {
            if (id != product.ID)
            {
                return BadRequest("Product ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); 
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }


        [HttpGet("Purchase")]
        public async Task<IActionResult> Purchase(int id,int quantity,int userId)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.Quantity < quantity)
            {
                return BadRequest("Product not available or insufficient quantity.");
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

            return Ok(new { message = "Purchase successful", transaction });
        }

    }
}
