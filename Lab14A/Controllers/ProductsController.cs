using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab14A.Models;
using Lab14A.Models.Requires;
using Lab14A.Models.Request;

namespace Lab14A.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly Context _context;

        public ProductsController(Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductList(REQDeleteProductList request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.Products == null)
                    {
                        return Problem("Entity set 'Context.Products' is null.");
                    }

                    var productsToDelete = await _context.Products
                        .Where(p => request.ProductIds.Contains(p.Id))
                        .ToListAsync();

                    if (productsToDelete == null || !productsToDelete.Any())
                    {
                        return NotFound("No products found for deletion.");
                    }


                    foreach (var product in productsToDelete)
                    {
                        product.Active = false;
                    }

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok("Products deactivated successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Problem($"Error: {ex.Message}");
                }
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductPrice(REQUpdateProductPrice request)
        {
            try
            {
                var product = await _context.Products.FindAsync(request.Id);

                if (product == null)
                {
                    return NotFound();
                }

                // Actualizar las propiedades según la solicitud
                product.Price = request.Price;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem($"Error: {ex.Message}");
            }
        }


        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            return await _context.Products.Where(x => x.Active == true).ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
          if (_context.Products == null)
          {
              return NotFound();
          }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(REQInsertProduct requestProduct)
        {
            try
            {
                var product = new Product
                {
                    Name = requestProduct.Name,
                    Price = requestProduct.Price,
                    Active = true
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return Problem($"Error: {ex.Message}");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(REQDeleteProduct requestProduct)
        {
            if (_context.Products == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(requestProduct.Id);

            if (product == null)
            {
                return NotFound();
            }
            product.Active = false;
            //_context.Entry(product).State = EntityState.Modified;
            //_context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
