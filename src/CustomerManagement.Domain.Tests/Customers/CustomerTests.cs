using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Shared;
using FluentAssertions;

namespace CustomerManagement.Domain.Tests.Customers
{
    public class CustomerTests
    {
        private Customer CreateValidCustomer()
        {
            var customer = Customer.Create("João Silva");
            customer.AddEmail("joao@example.com", true);
            customer.AddPhone(Phone.Create("11", "987654321", true));
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));
            customer.AddDocument(Document.Create("12345678901", "CPF"));
            return customer;
        }

        [Fact]
        public void Create_ValidName_ShouldCreateCustomer()
        {
            // Arrange
            string name = "Maria Souza";

            // Act
            var customer = Customer.Create(name);

            // Assert
            customer.Should().NotBeNull();
            customer.Name.Should().Be(name);
            customer.Id.Should().NotBeEmpty();
            customer.IsActive.Should().BeTrue();
            customer.Emails.Should().BeEmpty();
            customer.Phones.Should().BeEmpty();
            customer.Addresses.Should().BeEmpty();
            customer.Documents.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ab")] // Menos de 3 caracteres
        public void Create_InvalidName_ShouldThrowDomainException(string invalidName)
        {
            // Act
            Action act = () => Customer.Create(invalidName);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O nome do cliente deve ter pelo menos 3 caracteres.");
        }

        [Fact]
        public void UpdateName_ValidNewName_ShouldUpdateName()
        {
            // Arrange
            var customer = CreateValidCustomer();
            string newName = "João da Silva";

            // Act
            customer.UpdateName(newName);

            // Assert
            customer.Name.Should().Be(newName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ab")]
        public void UpdateName_InvalidNewName_ShouldThrowDomainException(string invalidName)
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.UpdateName(invalidName);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O nome do cliente deve ter pelo menos 3 caracteres.");
        }

        // Testes para Emails
        [Fact]
        public void AddEmail_NewEmail_ShouldAddEmail()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var newEmail = Email.Create("novo@example.com");

            // Act
            customer.AddEmail(newEmail.Value, false);

            // Assert
            customer.Emails.Should().Contain(newEmail);
            customer.Emails.Should().HaveCount(2);
        }

        [Fact]
        public void AddEmail_DuplicateEmail_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var existingEmail = Email.Create("joao@example.com");

            // Act
            Action act = () => customer.AddEmail(existingEmail.Value, false);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O e-mail '{existingEmail.Value}' já existe para este cliente.");
        }

        [Fact]
        public void AddEmail_NewPrimaryEmail_ShouldSetAsPrimaryAndDemoteOthers()
        {
            // Arrange
            var customer = CreateValidCustomer(); // joao@example.com é primário
            var newPrimaryEmail = Email.Create("novo_principal@example.com", true);

            // Act
            customer.AddEmail(newPrimaryEmail.Value, true);

            // Assert
            customer.Emails.Should().Contain(newPrimaryEmail);
            customer.Emails.Should().HaveCount(2);
            customer.Emails.Single(e => e.Value == newPrimaryEmail.Value).IsPrimary.Should().BeTrue();
            customer.Emails.Single(e => e.Value == "joao@example.com").IsPrimary.Should().BeFalse();
        }

        [Fact]
        public void RemoveEmail_ExistingEmail_ShouldRemoveEmail()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.AddEmail(Email.Create("secundario@example.com").Value, false);

            // Act
            customer.RemoveEmail("secundario@example.com");

            // Assert
            customer.Emails.Should().NotContain(e => e.Value == "secundario@example.com");
            customer.Emails.Should().HaveCount(1);
        }

        [Fact]
        public void RemoveEmail_NonExistingEmail_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.RemoveEmail("naoexiste@example.com");

            // Assert
            act.Should().Throw<DomainException>().WithMessage("E-mail 'naoexiste@example.com' não encontrado para remoção.");
        }

        [Fact]
        public void RemoveEmail_LastEmail_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer(); // Tem apenas 1 email

            // Act
            Action act = () => customer.RemoveEmail("joao@example.com");

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Não é possível remover o último e-mail do cliente.");
        }

        [Fact]
        public void SetPrimaryEmail_ExistingEmail_ShouldSetAsPrimaryAndDemoteOthers()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.AddEmail(Email.Create("secundario@example.com").Value, false); // joao@example.com é primário

            // Act
            customer.SetPrimaryEmail("secundario@example.com");

            // Assert
            customer.Emails.Single(e => e.Value == "secundario@example.com").IsPrimary.Should().BeTrue();
            customer.Emails.Single(e => e.Value == "joao@example.com").IsPrimary.Should().BeFalse();
        }

        [Fact]
        public void SetPrimaryEmail_NonExistingEmail_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.SetPrimaryEmail("naoexiste@example.com");

            // Assert
            act.Should().Throw<DomainException>().WithMessage("E-mail 'naoexiste@example.com' não encontrado para ser definido como principal.");
        }

        // Testes para Phones (similar aos emails)
        [Fact]
        public void AddPhone_NewPhone_ShouldAddPhone()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var newPhone = Phone.Create("21", "999998888");

            // Act
            customer.AddPhone(newPhone);

            // Assert
            customer.Phones.Should().Contain(newPhone);
            customer.Phones.Should().HaveCount(2);
        }

        [Fact]
        public void AddPhone_DuplicatePhone_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var existingPhone = Phone.Create("11", "987654321");

            // Act
            Action act = () => customer.AddPhone(existingPhone);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O telefone '({existingPhone.AreaCode}) {existingPhone.Number}' já existe para este cliente.");
        }

        [Fact]
        public void RemovePhone_LastPhone_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.RemovePhone("11", "987654321");

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Não é possível remover o último telefone do cliente.");
        }

        // Testes para Addresses (similar aos emails)
        [Fact]
        public void AddAddress_NewAddress_ShouldAddAddress()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var newAddress = Address.Create("Rua B", "20", "", "Cidade Y", "RJ", "22222-222", "Brasil");

            // Act
            customer.AddAddress(newAddress);

            // Assert
            customer.Addresses.Should().Contain(newAddress);
            customer.Addresses.Should().HaveCount(2);
        }

        [Fact]
        public void AddAddress_DuplicateAddress_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var existingAddress = Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil");

            // Act
            Action act = () => customer.AddAddress(existingAddress);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O endereço '{existingAddress}' já existe para este cliente.");
        }

        [Fact]
        public void RemoveAddress_LastAddress_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.RemoveAddress("Rua A", "10", "Cidade X");

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Não é possível remover o último endereço do cliente.");
        }

        // Testes para Documents
        [Fact]
        public void AddDocument_NewDocument_ShouldAddDocument()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var newDocument = Document.Create("22233344455", "CPF");

            // Act
            customer.AddDocument(newDocument);

            // Assert
            customer.Documents.Should().Contain(newDocument);
            customer.Documents.Should().HaveCount(2);
        }

        [Fact]
        public void AddDocument_DuplicateDocument_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();
            var existingDocument = Document.Create("12345678901", "CPF");

            // Act
            Action act = () => customer.AddDocument(existingDocument);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O documento '{existingDocument.Number}' do tipo '{existingDocument.Type}' já existe para este cliente.");
        }

        [Fact]
        public void RemoveDocument_ExistingDocument_ShouldRemoveDocument()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.AddDocument(Document.Create("22233344455", "CPF"));

            // Act
            customer.RemoveDocument("22233344455");

            // Assert
            customer.Documents.Should().NotContain(d => d.Number == "22233344455");
            customer.Documents.Should().HaveCount(1);
        }

        [Fact]
        public void RemoveDocument_NonExistingDocument_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.RemoveDocument("99999999999");

            // Assert
            act.Should().Throw<DomainException>().WithMessage("Documento '99999999999' não encontrado para remoção.");
        }

        // Testes para Ativação/Desativação
        [Fact]
        public void Deactivate_ActiveCustomer_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            customer.Deactivate();

            // Assert
            customer.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Deactivate_InactiveCustomer_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.Deactivate(); // Já inativo

            // Act
            Action act = () => customer.Deactivate();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente já está inativo.");
        }

        [Fact]
        public void Activate_InactiveCustomer_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.Deactivate();

            // Act
            customer.Activate();

            // Assert
            customer.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Activate_ActiveCustomer_ShouldThrowDomainException()
        {
            // Arrange
            var customer = CreateValidCustomer(); // Já ativo

            // Act
            Action act = () => customer.Activate();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente já está ativo.");
        }

        // Testes de validação de estado do cliente
        [Fact]
        public void ValidateCustomerState_ValidCustomer_ShouldNotThrowException()
        {
            // Arrange
            var customer = CreateValidCustomer();

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().NotThrow<DomainException>();
        }

        [Fact]
        public void ValidateCustomerState_NoEmails_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddPhone(Phone.Create("11", "987654321", true));
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));
            customer.AddDocument(Document.Create("12345678901", "CPF"));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter pelo menos um e-mail.");
        }

        [Fact]
        public void ValidateCustomerState_NoPrimaryEmail_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddEmail(Email.Create("email@example.com", false).Value, false); // Não primário
            customer.AddPhone(Phone.Create("11", "987654321", true));
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));
            customer.AddDocument(Document.Create("12345678901", "CPF"));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter um e-mail principal.");
        }

        [Fact]
        public void ValidateCustomerState_NoPhones_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddEmail(Email.Create("email@example.com", true).Value, true);
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));
            customer.AddDocument(Document.Create("12345678901", "CPF"));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter pelo menos um telefone.");
        }

        [Fact]
        public void ValidateCustomerState_NoPrimaryPhone_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddEmail("email@example.com", true);
            customer.AddPhone(Phone.Create("11", "987654321", false)); // Não primário
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));
            customer.AddDocument(Document.Create("12345678901", "CPF"));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter um telefone principal.");
        }

        [Fact]
        public void ValidateCustomerState_NoAddresses_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddEmail(Email.Create("email@example.com", true).Value, true);
            customer.AddPhone(Phone.Create("11", "987654321", true));
            customer.AddDocument(Document.Create("12345678901", "CPF"));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter pelo menos um endereço.");
        }

        [Fact]
        public void ValidateCustomerState_NoPrimaryAddress_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddEmail(Email.Create("email@example.com", true).Value, true);
            customer.AddPhone(Phone.Create("11", "987654321", true));
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", false)); // Não primário
            customer.AddDocument(Document.Create("12345678901", "CPF"));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter um endereço principal.");
        }

        [Fact]
        public void ValidateCustomerState_NoDocuments_ShouldThrowException()
        {
            // Arrange
            var customer = Customer.Create("Teste");
            customer.AddEmail(Email.Create("email@example.com", true).Value, true);
            customer.AddPhone(Phone.Create("11", "987654321", true));
            customer.AddAddress(Address.Create("Rua A", "10", "", "Cidade X", "SP", "11111-111", "Brasil", true));

            // Act
            Action act = () => customer.ValidateCustomerState();

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O cliente deve ter pelo menos um documento.");
        }
    }
}