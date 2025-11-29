using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Repositories;
using CustomerManagement.Domain.Shared;
using FluentAssertions;

namespace CustomerManagement.Domain.Tests.Customers
{
    public class InMemoryCustomerRepositoryTests
    {
        private InMemoryCustomerRepository _repository;

        public InMemoryCustomerRepositoryTests()
        {
            _repository = new InMemoryCustomerRepository();
        }

        private Customer CreateValidCustomer(string name, string email, string phoneArea, string phoneNumber, string street, string docNumber)
        {
            var customer = Customer.Create(name);
            customer.AddEmail(email, true);
            customer.AddPhone(Phone.Create(phoneArea, phoneNumber, true));
            customer.AddAddress(Address.Create(street, "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));
            customer.AddDocument(Document.Create(docNumber, "CPF"));
            return customer;
        }

        [Fact]
        public async Task Add_NewCustomer_ShouldAddSuccessfully()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");

            // Act
            await _repository.Add(customer);

            // Assert
            var retrievedCustomer = await _repository.GetById(customer.Id);
            retrievedCustomer.Should().NotBeNull();
            retrievedCustomer.Name.Should().Be("Cliente Teste 1");
            _repository.GetAll().Result.Should().HaveCount(1);
        }

        [Fact]
        public async Task Add_CustomerWithExistingDocument_ShouldThrowDomainException()
        {
            // Arrange
            var customer1 = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer1);
            var customer2 = CreateValidCustomer("Cliente Teste 2", "teste2@email.com", "22", "933334444", "Rua B", "11122233344"); // Mesmo documento

            // Act
            Action act = async () => await _repository.Add(customer2);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Já existe um cliente com o documento '11122233344'.");
            _repository.GetAll().Result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetById_ExistingCustomer_ShouldReturnCustomer()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer);

            // Act
            var retrievedCustomer = await _repository.GetById(customer.Id);

            // Assert
            retrievedCustomer.Should().Be(customer);
        }

        [Fact]
        public async Task GetById_NonExistingCustomer_ShouldReturnNull()
        {
            // Act
            var retrievedCustomer = await _repository.GetById(Guid.NewGuid());

            // Assert
            retrievedCustomer.Should().BeNull();
        }

        [Fact]
        public async Task GetByDocument_ExistingDocument_ShouldReturnCustomer()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer);

            // Act
            var retrievedCustomer = await _repository.GetByDocument("11122233344");

            // Assert
            retrievedCustomer.Should().Be(customer);
        }

        [Fact]
        public async Task GetByDocument_NonExistingDocument_ShouldReturnNull()
        {
            // Act
            var retrievedCustomer = await _repository.GetByDocument("99999999999");

            // Assert
            retrievedCustomer.Should().BeNull();
        }

        [Fact]
        public async Task Update_ExistingCustomer_ShouldUpdateSuccessfully()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Original", "original@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer);
            customer.UpdateName("Cliente Atualizado");

            // Act
            await _repository.Update(customer);

            // Assert
            var updatedCustomer = await _repository.GetById(customer.Id);
            updatedCustomer.Name.Should().Be("Cliente Atualizado");
        }

        [Fact]
        public async Task Update_NonExistingCustomer_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Inexistente", "inexistente@email.com", "11", "911112222", "Rua A", "11122233344");

            // Act
            Action act = async () => await _repository.Update(customer);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"Cliente com ID '{customer.Id}' não encontrado para atualização.");
        }

        [Fact]
        public async Task Update_CustomerDocumentChangedToExistingOne_ShouldThrowDomainException()
        {
            // Arrange
            var customer1 = CreateValidCustomer("Cliente 1", "email1@email.com", "11", "911112222", "Rua A", "11122233344");
            var customer2 = CreateValidCustomer("Cliente 2", "email2@email.com", "22", "933334444", "Rua B", "55566677788");
            await _repository.Add(customer1);
            await _repository.Add(customer2);

            // Modifica o documento do cliente2 para ser igual ao do cliente1
            customer2.AddDocument(Document.Create("11122233344", "CPF")); // Documento do cliente1
            
            // Act
            Action act = async () => await _repository.Update(customer2);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O documento '11122233344' já está associado a outro cliente.");
        }

        [Fact]
        public async Task Delete_ExistingCustomer_ShouldDeleteSuccessfully()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer);

            // Act
            await _repository.Delete(customer.Id);

            // Assert
            _repository.GetById(customer.Id).Result.Should().BeNull();
            _repository.GetAll().Result.Should().BeEmpty();
        }

        [Fact]
        public async Task Delete_NonExistingCustomer_ShouldThrowDomainException()
        {
            // Act
            Action act = async () => await _repository.Delete(Guid.NewGuid());

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Cliente com ID '*' não encontrado para exclusão.");
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCustomers()
        {
            // Arrange
            await _repository.Add(CreateValidCustomer("Cliente 1", "email1@email.com", "11", "911112222", "Rua A", "11122233344"));
            await _repository.Add(CreateValidCustomer("Cliente 2", "email2@email.com", "22", "933334444", "Rua B", "55566677788"));

            // Act
            var customers = await _repository.GetAll();

            // Assert
            customers.Should().HaveCount(2);
        }

        [Fact]
        public async Task DocumentExists_ExistingDocument_ShouldReturnTrue()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer);

            // Act
            var exists = await _repository.DocumentExists("11122233344");

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task DocumentExists_NonExistingDocument_ShouldReturnFalse()
        {
            // Act
            var exists = await _repository.DocumentExists("99999999999");

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task CustomerExists_ExistingCustomer_ShouldReturnTrue()
        {
            // Arrange
            var customer = CreateValidCustomer("Cliente Teste 1", "teste1@email.com", "11", "911112222", "Rua A", "11122233344");
            await _repository.Add(customer);

            // Act
            var exists = await _repository.CustomerExists(customer.Id);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task CustomerExists_NonExistingCustomer_ShouldReturnFalse()
        {
            // Act
            var exists = await _repository.CustomerExists(Guid.NewGuid());

            // Assert
            exists.Should().BeFalse();
        }
    }
}