using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab14A.Models;
using Lab14A.Models.Request;
using Lab14A.Models.Requires;

namespace Lab14A.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly Context _context;

        public CustomersController(Context context)
        {
            _context = context;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomerDocument(REQUpdateCustomerDocument request)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(request.Id);

                if (customer == null)
                {
                    return NotFound();
                }

                // Actualizar las propiedades según la solicitud
                customer.DocumentNumber = request.DocumentNumber;
                customer.Email = request.Email;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem($"Error: {ex.Message}");
            }
        }


        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            return await _context.Customers.Where(x => x.Active == true).ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
          if (_context.Customers == null)
          {
              return NotFound();
          }
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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


        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(REQInsertCustomer requestCustomer)
        {
            try
            {
                var customer = new Customer
                {
                    FirstName = requestCustomer.FirstName,
                    LastName = requestCustomer.LastName,
                    DocumentNumber = requestCustomer.DocumentNumber,
                    Email = requestCustomer.Email,
                    Active = true
                };

                if (_context.Customers == null)
                {
                    return Problem("Entity set 'Context.Customers' is null.");
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
            }
            catch (Exception ex)
            {
                return Problem($"Error: {ex.Message}");
            }
        }


        // DELETE: api/Customers/5
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(REQDeleteCustomer requestCustomer)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(requestCustomer.Id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Active = false;
            //_context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }
    }
}
