using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Repositories;
using CustomerManagement.Domain.Shared;
using CustomerManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de clientes para SQL Server usando Entity Framework Core.
    /// </summary>
    public class SqlServerCustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        /// <summary>
        /// Construtor que recebe o CustomerDbContext via injeção de dependência.
        /// </summary>
        /// <param name="context">O contexto de banco de dados para clientes.</param>
        public SqlServerCustomerRepository(CustomerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adiciona um novo cliente ao banco de dados.
        /// Antes de adicionar, verifica se já existe um cliente com o mesmo número de documento.
        /// </summary>
        /// <param name="customer">O cliente a ser adicionado.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        public async Task Add(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza um cliente existente no banco de dados.
        /// </summary>
        /// <param name="customer">O cliente a ser atualizado.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        public async Task Update(Customer customer)
        {
            // Anexa o cliente ao contexto e marca como modificado.
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove um cliente do banco de dados pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do cliente a ser removido.</param>
        /// <returns>Uma Task que representa a operação assíncrona.</returns>
        public async Task Delete(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Busca um cliente pelo seu ID, incluindo todas as suas coleções de Value Objects.
        /// </summary>
        /// <param name="id">O ID do cliente.</param>
        /// <returns>Uma Task que retorna o cliente encontrado ou null se não existir.</returns>
        public async Task<Customer?> GetById(Guid id)
        {
            return await _context.Customers
                .Include(c => c.Emails)
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Retorna todos os clientes do banco de dados, incluindo suas coleções de Value Objects.
        /// </summary>
        /// <returns>Uma Task que retorna uma coleção de todos os clientes.</returns>
        public async Task<List<Customer>> GetAll()
        {
            return await _context.Customers
                .Include(c => c.Emails)
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .ToListAsync();
        }

        /// <summary>
        /// Busca um cliente pelo número do documento, incluindo suas coleções de Value Objects.
        /// </summary>
        /// <param name="documentNumber">O número do documento do cliente.</param>
        /// <returns>Uma Task que retorna o cliente encontrado ou null se não existir.</returns>
        public async Task<Customer?> GetByDocument(string documentNumber)
        {
            return await _context.Customers
                .Include(c => c.Emails)
                .Include(c => c.Phones)
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Documents.Any(d => d.Number == documentNumber));
        }

        public async Task<bool> CustomerExists(Guid customerId)
        {
            return await _context.Customers.AnyAsync(c => c.Id == customerId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}