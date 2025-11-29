using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Shared;
using System.Runtime.CompilerServices;

namespace CustomerManagement.Domain.Repositories
{
    // Esta implementação é para fins de teste e demonstração em memória.
    // Em um ambiente real, seria substituída por uma implementação de banco de dados.
    public class InMemoryCustomerRepository : ICustomerRepository
    {
        private readonly List<Customer> _customers;

        public InMemoryCustomerRepository()
        {
            _customers = new List<Customer>();
        }

        public async Task Add(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Cliente não pode ser nulo.");
            }

            // Valida se já existe um cliente com o mesmo documento
            foreach (var doc in customer.Documents)
            {
                if (await DocumentExists(doc.Number))
                {
                    throw new DomainException($"Já existe um cliente com o documento '{doc.Number}'.");
                }
            }

            // Valida o estado do cliente antes de adicionar
            customer.ValidateCustomerState();

            _customers.Add(customer);
        }

        public async Task Update(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Cliente não pode ser nulo.");
            }

            var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
            if (existingCustomer == null)
            {
                throw new DomainException($"Cliente com ID '{customer.Id}' não encontrado para atualização.");
            }

            // Valida se algum documento foi alterado para um já existente em outro cliente
            foreach (var doc in customer.Documents)
            {
                var customerWithSameDoc = _customers.FirstOrDefault(c => c.Id != customer.Id && c.Documents.Any(d => d.Number == doc.Number));
                if (customerWithSameDoc != null)
                {
                    throw new DomainException($"O documento '{doc.Number}' já está associado a outro cliente.");
                }
            }

            // Valida o estado do cliente antes de atualizar
            customer.ValidateCustomerState();

            _customers.Remove(existingCustomer);
            _customers.Add(customer);
        }

        public async Task Delete(Guid customerId)
        {
            var customerToRemove = _customers.FirstOrDefault(c => c.Id == customerId);
            if (customerToRemove == null)
            {
                throw new DomainException($"Cliente com ID '{customerId}' não encontrado para exclusão.");
            }
            _customers.Remove(customerToRemove);
        }

        public async Task<Customer> GetById(Guid customerId)
        {
            return _customers.FirstOrDefault(c => c.Id == customerId);
        }

        public async Task<Customer> GetByDocument(string documentNumber)
        {
            return _customers.FirstOrDefault(c => c.Documents.Any(d => d.Number == documentNumber));
        }

        public async Task<List<Customer>> GetAll()
        {
            return new List<Customer>(_customers); // Retorna uma cópia para evitar modificações externas
        }

        public async Task<bool> DocumentExists(string documentNumber)
        {
            return _customers.Any(c => c.Documents.Any(d => d.Number == documentNumber));
        }

        public async Task<bool> CustomerExists(Guid customerId)
        {
            return _customers.Any(c => c.Id == customerId);
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}