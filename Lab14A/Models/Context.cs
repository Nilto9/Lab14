using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lab14A.Models
{
    public class Context : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Detail> Details { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {

        }

    }
}
