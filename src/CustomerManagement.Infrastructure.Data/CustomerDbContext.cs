using CustomerManagement.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Infrastructure.Data
{
    /// <summary>
    /// Contexto de banco de dados para a gestão de clientes, utilizando Entity Framework Core.
    /// </summary>
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Representa a coleção de clientes no banco de dados.
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(e => e.Id);

                e.OwnsMany(e => e.Addresses);

                e.OwnsMany(e => e.Documents);

                e.OwnsMany(e => e.Phones);

                e.OwnsMany(e => e.Emails);
            });
        }
    }
}