using CustomerManagement.Domain.Customers;

namespace CustomerManagement.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task Add(Customer customer);
        
        Task Update(Customer customer);
        
        Task Delete(Guid customerId);
        
        Task<Customer> GetById(Guid customerId);
        
        Task<Customer> GetByDocument(string documentNumber);
        
        Task<List<Customer>> GetAll();
        
        Task<bool> CustomerExists(Guid customerId);
        Task<int> SaveChangesAsync();
    }
}