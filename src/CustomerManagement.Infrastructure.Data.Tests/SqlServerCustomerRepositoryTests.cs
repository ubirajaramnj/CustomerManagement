using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Shared;
using CustomerManagement.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Infrastructure.Data.Tests
{
    // A classe de testes para o SqlServerCustomerRepository.
    // Utiliza IClassFixture para compartilhar o setup do DbContext em memória entre os testes,
    // garantindo que cada teste comece com um estado limpo.
    public class SqlServerCustomerRepositoryTests : IClassFixture<CustomerRepositoryTestFixture>
    {
        private readonly CustomerRepositoryTestFixture _fixture;
        private readonly SqlServerCustomerRepository _repository;
        private readonly CustomerDbContext _context;

        public SqlServerCustomerRepositoryTests(CustomerRepositoryTestFixture fixture)
        {
            _fixture = fixture;
            // Para cada teste, obtemos um novo contexto e repositório para garantir isolamento.
            // O fixture garante que o contexto seja resetado (limpo) antes de cada teste.

            _fixture.InitializeAsync().Wait();

            _context = _fixture.DbContext;
            _repository = new SqlServerCustomerRepository(_context);
        }

        // Helper para criar um cliente de teste
        private Customer CreateTestCustomer(
            string documentNumber,
            string name = "Test Customer",
            bool isPrimaryEmail = true,
            bool isPrimaryPhone = true,
            bool isPrimaryAddress = true)
        {
            var document = Document.Create(documentNumber, "CPF");
            var email = Email.Create($"test.{documentNumber}@example.com", isPrimaryEmail);
            var phone = Phone.Create("11", "987654321", isPrimaryPhone);
            var address = Address.Create("Rua Teste", "123", "Apto 1", "Cidade Teste", "SP", "01000-000", "Brasil", isPrimaryAddress);

            var customer = Customer.Create(name);
            customer.AddEmail(email.Value, email.IsPrimary);
            customer.AddPhone(phone);
            customer.AddAddress(address);

            return customer;
        }

        // 1. Teste: Adicionar novo cliente com sucesso
        [Fact]
        public async Task AddAsync_WithValidCustomer_ShouldAddSuccessfully()
        {
            // Arrange
            var customer = CreateTestCustomer("111.111.111-11");

            // Act
            await _repository.Add(customer);
            await _context.SaveChangesAsync(); // Salva as mudanças no contexto

            // Assert
            var savedCustomer = await _context.Customers
                                            .Include(c => c.Emails)
                                            .Include(c => c.Phones)
                                            .Include(c => c.Addresses)
                                            .Include(c => c.Documents)
                                            .FirstOrDefaultAsync(c => c.Id == customer.Id);

            savedCustomer.Should().NotBeNull();
            savedCustomer.Name.Should().Be(customer.Name);
            savedCustomer.Documents.First().Number.Should().Be(customer.Documents.First().Number);
            savedCustomer.Emails.Should().ContainSingle(e => e.Value == customer.Emails.First().Value);
            savedCustomer.Phones.Should().ContainSingle(p => p.Number == customer.Phones.First().Number);
            savedCustomer.Addresses.Should().ContainSingle(a => a.Street == customer.Addresses.First().Street);
        }

        // 2. Teste: Lançar exceção quando documento já existe
        [Fact]
        public async Task AddAsync_WithDuplicateDocument_ShouldThrowException()
        {
            // Arrange
            var documentNumber = "222.222.222-22";
            var customer1 = CreateTestCustomer(documentNumber);
            await _repository.Add(customer1);
            await _context.SaveChangesAsync();

            var customer2 = CreateTestCustomer(documentNumber); // Cliente com o mesmo documento

            // Act & Assert
            // Espera-se que o método Add lance uma DomainException
            await _repository.Invoking(r => r.Add(customer2))
                             .Should().ThrowAsync<DomainException>()
                             .WithMessage("Já existe um cliente com o documento '222.222.222-22'.");
        }

        // 3. Teste: Retornar cliente quando existe
        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnCustomer()
        {
            // Arrange
            var customer = CreateTestCustomer("333.333.333-33");
            await _repository.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var foundCustomer = await _repository.GetById(customer.Id);

            // Assert
            foundCustomer.Should().NotBeNull();
            foundCustomer.Id.Should().Be(customer.Id);
            foundCustomer.Name.Should().Be(customer.Name);
        }

        // 4. Teste: Retornar null quando não existe
        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var foundCustomer = await _repository.GetById(nonExistingId);

            // Assert
            foundCustomer.Should().BeNull();
        }

        // 5. Teste: Retornar todos os clientes
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers()
        {
            // Arrange
            var customer1 = CreateTestCustomer("444.444.444-44");
            var customer2 = CreateTestCustomer("555.555.555-55");
            await _repository.Add(customer1);
            await _repository.Add(customer2);
            await _context.SaveChangesAsync();

            // Act
            var customers = await _repository.GetAll();

            // Assert
            customers.Should().NotBeNull();
            customers.Should().HaveCount(2);
            customers.Should().Contain(c => c.Id == customer1.Id);
            customers.Should().Contain(c => c.Id == customer2.Id);
        }

        // 6. Teste: Retornar lista vazia quando nenhum cliente existe
        [Fact]
        public async Task GetAllAsync_WithNoCustomers_ShouldReturnEmptyList()
        {
            // Arrange - O contexto já está limpo pelo fixture

            // Act
            var customers = await _repository.GetAll();

            // Assert
            customers.Should().NotBeNull();
            customers.Should().BeEmpty();
        }

        // 7. Teste: Atualizar cliente existente
        [Fact]
        public async Task UpdateAsync_WithExistingCustomer_ShouldUpdateSuccessfully()
        {
            // Arrange
            var customer = CreateTestCustomer("666.666.666-66");
            await _repository.Add(customer);
            await _context.SaveChangesAsync();

            // Modifica o cliente
            var newName = "Updated Customer Name";
            customer.UpdateName(newName);
            customer.AddEmail("new.email@example.com", false); // Adiciona um novo email

            // Act
            await _repository.Update(customer);
            await _context.SaveChangesAsync();

            // Assert
            var updatedCustomer = await _context.Customers
                                                .Include(c => c.Emails)
                                                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            updatedCustomer.Should().NotBeNull();
            updatedCustomer.Name.Should().Be(newName);
            updatedCustomer.Emails.Should().ContainSingle(e => e.Value == "new.email@example.com");
        }

        // 8. Teste: Deletar cliente existente
        [Fact]
        public async Task DeleteAsync_WithExistingId_ShouldDeleteSuccessfully()
        {
            // Arrange
            var customer = CreateTestCustomer("777.777.777-77");
            await _repository.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            await _repository.Delete(customer.Id);
            await _context.SaveChangesAsync();

            // Assert
            var deletedCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customer.Id);
            deletedCustomer.Should().BeNull();
        }

        // 9. Teste: Não lançar erro ao deletar cliente inexistente
        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldNotThrow()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act & Assert
            // A operação de deletar um ID que não existe não deve lançar exceção
            await _repository.Invoking(r => r.Delete(nonExistingId))
                             .Should().NotThrowAsync();
        }

        // 10. Teste: Retornar cliente por documento
        [Fact]
        public async Task GetByDocumentAsync_WithExistingDocument_ShouldReturnCustomer()
        {
            // Arrange
            var documentNumber = "888.888.888-88";
            var customer = CreateTestCustomer(documentNumber);
            await _repository.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var foundCustomer = await _repository.GetByDocument(documentNumber);

            // Assert
            foundCustomer.Should().NotBeNull();
            foundCustomer.Documents.First().Number.Should().Be(documentNumber);
        }

        // 11. Teste: Retornar null quando documento não existe
        [Fact]
        public async Task GetByDocumentAsync_WithNonExistingDocument_ShouldReturnNull()
        {
            // Arrange
            var nonExistingDocument = "999.999.999-99";

            // Act
            var foundCustomer = await _repository.GetByDocument(nonExistingDocument);

            // Assert
            foundCustomer.Should().BeNull();
        }
    }
}