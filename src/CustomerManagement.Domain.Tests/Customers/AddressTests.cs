using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Shared;
using FluentAssertions;

namespace CustomerManagement.Domain.Tests.Customers
{
    public class AddressTests
    {
        [Fact]
        public void Create_ValidAddress_ShouldReturnAddressObject()
        {
            // Arrange
            string street = "Rua Teste";
            string number = "123";
            string complement = "Apto 101";
            string city = "São Paulo";
            string state = "SP";
            string zipCode = "01000-000";
            string country = "Brasil";

            // Act
            var address = Address.Create(street, number, complement, city, state, zipCode, country);

            // Assert
            address.Should().NotBeNull();
            address.Street.Should().Be(street);
            address.Number.Should().Be(number);
            address.Complement.Should().Be(complement);
            address.City.Should().Be(city);
            address.State.Should().Be(state);
            address.ZipCode.Should().Be(zipCode);
            address.Country.Should().Be(country);
            address.IsPrimary.Should().BeFalse();
        }

        [Fact]
        public void Create_ValidAddressAsPrimary_ShouldReturnPrimaryAddressObject()
        {
            // Arrange
            string street = "Rua Principal";
            string number = "456";
            string complement = "";
            string city = "Rio de Janeiro";
            string state = "RJ";
            string zipCode = "20000-000";
            string country = "Brasil";

            // Act
            var address = Address.Create(street, number, complement, city, state, zipCode, country, true);

            // Assert
            address.Should().NotBeNull();
            address.Street.Should().Be(street);
            address.Number.Should().Be(number);
            address.IsPrimary.Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "123", "Cidade", "Estado", "CEP", "País", "A rua não pode ser vazia.")]
        [InlineData("Rua", null, "Cidade", "Estado", "CEP", "País", "O número do endereço não pode ser vazio.")]
        [InlineData("Rua", "123", null, "Estado", "CEP", "País", "A cidade não pode ser vazia.")]
        [InlineData("Rua", "123", "Cidade", null, "CEP", "País", "O estado não pode ser vazio.")]
        [InlineData("Rua", "123", "Cidade", "Estado", null, "País", "O CEP não pode ser vazio.")]
        [InlineData("Rua", "123", "Cidade", "Estado", "CEP", null, "O país não pode ser vazio.")]
        public void Create_MissingRequiredField_ShouldThrowDomainException(string street, string number, string city, string state, string zipCode, string country, string expectedMessage)
        {
            // Act
            Action act = () => Address.Create(street, number, "Complemento", city, state, zipCode, country);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void Equals_TwoAddressesWithSameValues_ShouldReturnTrue()
        {
            // Arrange
            var address1 = Address.Create("Rua A", "10", "Apto 1", "Cidade X", "UF", "11111-111", "Brasil");
            var address2 = Address.Create("Rua A", "10", "Apto 1", "Cidade X", "UF", "11111-111", "Brasil");

            // Act & Assert
            address1.Should().Be(address2);
            (address1 == address2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoAddressesWithDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var address1 = Address.Create("Rua A", "10", "Apto 1", "Cidade X", "UF", "11111-111", "Brasil");
            var address2 = Address.Create("Rua B", "20", "Apto 2", "Cidade Y", "UF", "22222-222", "Brasil");

            // Act & Assert
            address1.Should().NotBe(address2);
            (address1 != address2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoAddressesWithSameValuesButDifferentComplement_ShouldReturnTrue()
        {
            // Arrange
            var address1 = Address.Create("Rua A", "10", "Apto 1", "Cidade X", "UF", "11111-111", "Brasil");
            var address2 = Address.Create("Rua A", "10", "Apto 2", "Cidade X", "UF", "11111-111", "Brasil");

            // Act & Assert
            address1.Should().Be(address2); // Complemento não faz parte dos componentes de igualdade
            (address1 == address2).Should().BeTrue();
        }

        [Fact]
        public void SetAsPrimary_ShouldReturnNewAddressWithIsPrimaryTrue()
        {
            // Arrange
            var address = Address.Create("Rua Teste", "123", "", "Cidade", "UF", "12345-678", "Brasil", false);

            // Act
            var primaryAddress = address.SetAsPrimary();

            // Assert
            primaryAddress.IsPrimary.Should().BeTrue();
            address.IsPrimary.Should().BeFalse(); // Original deve permanecer imutável
            primaryAddress.Street.Should().Be(address.Street);
        }

        [Fact]
        public void SetAsSecondary_ShouldReturnNewAddressWithIsPrimaryFalse()
        {
            // Arrange
            var address = Address.Create("Rua Teste", "123", "", "Cidade", "UF", "12345-678", "Brasil", true);

            // Act
            var secondaryAddress = address.SetAsSecondary();

            // Assert
            secondaryAddress.IsPrimary.Should().BeFalse();
            address.IsPrimary.Should().BeTrue(); // Original deve permanecer imutável
            secondaryAddress.Street.Should().Be(address.Street);
        }
    }
}