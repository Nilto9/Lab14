using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab14A.Models;
using Lab14A.Models.Request;

namespace Lab14A.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly Context _context;

        public InvoicesController(Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> InsertInvoiceDetail(REQInsertInvoiceDetail request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var invoice = await _context.Invoices.FindAsync(request.IdInvoice);

                    if (invoice == null)
                    {
                        return NotFound($"Invoice with ID {request.IdInvoice} not found.");
                    }

                    if (_context.Details == null || _context.Products == null)
                    {
                        return Problem("Entity sets 'Context.Details' or 'Context.Products' are null.");
                    }

                    foreach (var reqDetail in request.Details)
                    {
                        var product = await _context.Products.FindAsync(reqDetail.ProductId);

                        if (product == null)
                        {
                            return NotFound($"Product with ID {reqDetail.ProductId} not found.");
                        }

                        var detail = new Detail
                        {
                            ProductId = reqDetail.ProductId,
                            Amount = reqDetail.Amount,
                            Price = reqDetail.Price,
                            Subtotal = reqDetail.Amount * reqDetail.Price,
                            Product = product,  // Asociar el producto al detalle
                            Invoice = invoice    // Asociar la factura al detalle
                        };

                        _context.Details.Add(detail);
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return NoContent();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Problem($"Error: {ex.Message}");
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> InsertInvoiceList(REQInsertInvoiceList request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var customer = await _context.Customers.FindAsync(request.IdCustomer);

                    if (customer == null)
                    {
                        return NotFound($"Customer with ID {request.IdCustomer} not found.");
                    }

                    if (_context.Invoices == null)
                    {
                        return Problem("Entity set 'Context.Invoices' is null.");
                    }

                    foreach (var reqInvoice in request.Invoices)
                    {
                        var invoice = new Invoice
                        {
                            CustomerId = request.IdCustomer,
                            Date = reqInvoice.Date,
                            InvoiceNumber = reqInvoice.InvoiceNumber,
                            Total = reqInvoice.Total
                        };

                        _context.Invoices.Add(invoice);
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return NoContent();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Problem($"Error: {ex.Message}");
                }
            }
        }


        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
          if (_context.Invoices == null)
          {
              return NotFound();
          }
            return await _context.Invoices.ToListAsync();
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
          if (_context.Invoices == null)
          {
              return NotFound();
          }
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // PUT: api/Invoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
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

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(REQInsertInvoice requestInvoice)
        {
            try
            {
                var invoice = new Invoice
                {
                    Date = requestInvoice.Date,
                    InvoiceNumber = requestInvoice.InvoiceNumber,
                    Total = requestInvoice.Total,
                    CustomerId = requestInvoice.IdCustomer,
                };

                if (_context.Invoices == null)
                {
                    return Problem("Entity set 'Context.Invoices' is null.");
                }

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
            }
            catch (Exception ex)
            {
                return Problem($"Error: {ex.Message}");
            }
        }


        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            if (_context.Invoices == null)
            {
                return NotFound();
            }
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            return (_context.Invoices?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
