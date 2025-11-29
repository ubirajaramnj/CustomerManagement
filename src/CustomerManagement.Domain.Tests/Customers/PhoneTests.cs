using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Shared;
using FluentAssertions;

namespace CustomerManagement.Domain.Tests.Customers
{
    public class PhoneTests
    {
        [Fact]
        public void Create_ValidPhone_ShouldReturnPhoneObject()
        {
            // Arrange
            string areaCode = "11";
            string number = "987654321";

            // Act
            var phone = Phone.Create(areaCode, number);

            // Assert
            phone.Should().NotBeNull();
            phone.AreaCode.Should().Be(areaCode);
            phone.Number.Should().Be(number);
            phone.IsPrimary.Should().BeFalse();
        }

        [Fact]
        public void Create_ValidPhoneAsPrimary_ShouldReturnPrimaryPhoneObject()
        {
            // Arrange
            string areaCode = "21";
            string number = "123456789";

            // Act
            var phone = Phone.Create(areaCode, number, true);

            // Assert
            phone.Should().NotBeNull();
            phone.AreaCode.Should().Be(areaCode);
            phone.Number.Should().Be(number);
            phone.IsPrimary.Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "123456789")]
        [InlineData("", "123456789")]
        [InlineData(" ", "123456789")]
        public void Create_NullOrEmptyAreaCode_ShouldThrowDomainException(string invalidAreaCode, string number)
        {
            // Act
            Action act = () => Phone.Create(invalidAreaCode, number);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O código de área não pode ser vazio.");
        }

        [Theory]
        [InlineData("1", "123456789")] // Menos de 2 dígitos
        [InlineData("123", "123456789")] // Mais de 2 dígitos
        [InlineData("AB", "123456789")] // Não numérico
        public void Create_InvalidAreaCodeFormat_ShouldThrowDomainException(string invalidAreaCode, string number)
        {
            // Act
            Action act = () => Phone.Create(invalidAreaCode, number);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O código de área '{invalidAreaCode}' deve ter 2 dígitos numéricos.");
        }

        [Theory]
        [InlineData("11", null)]
        [InlineData("11", "")]
        [InlineData("11", " ")]
        public void Create_NullOrEmptyNumber_ShouldThrowDomainException(string areaCode, string invalidNumber)
        {
            // Act
            Action act = () => Phone.Create(areaCode, invalidNumber);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O número de telefone não pode ser vazio.");
        }

        [Theory]
        [InlineData("11", "1234567")] // Menos de 8 dígitos
        [InlineData("11", "1234567890")] // Mais de 9 dígitos
        [InlineData("11", "1234567A")] // Não numérico
        public void Create_InvalidNumberFormat_ShouldThrowDomainException(string areaCode, string invalidNumber)
        {
            // Act
            Action act = () => Phone.Create(areaCode, invalidNumber);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O número de telefone '{invalidNumber}' deve ter entre 8 e 9 dígitos numéricos.");
        }

        [Fact]
        public void Equals_TwoPhonesWithSameAreaCodeAndNumber_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = Phone.Create("11", "999999999");
            var phone2 = Phone.Create("11", "999999999");

            // Act & Assert
            phone1.Should().Be(phone2);
            (phone1 == phone2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoPhonesWithDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var phone1 = Phone.Create("11", "999999999");
            var phone2 = Phone.Create("21", "888888888");

            // Act & Assert
            phone1.Should().NotBe(phone2);
            (phone1 != phone2).Should().BeTrue();
        }

        [Fact]
        public void SetAsPrimary_ShouldReturnNewPhoneWithIsPrimaryTrue()
        {
            // Arrange
            var phone = Phone.Create("11", "987654321", false);

            // Act
            var primaryPhone = phone.SetAsPrimary();

            // Assert
            primaryPhone.IsPrimary.Should().BeTrue();
            phone.IsPrimary.Should().BeFalse(); // Original deve permanecer imutável
            primaryPhone.AreaCode.Should().Be(phone.AreaCode);
            primaryPhone.Number.Should().Be(phone.Number);
        }

        [Fact]
        public void SetAsSecondary_ShouldReturnNewPhoneWithIsPrimaryFalse()
        {
            // Arrange
            var phone = Phone.Create("11", "987654321", true);

            // Act
            var secondaryPhone = phone.SetAsSecondary();

            // Assert
            secondaryPhone.IsPrimary.Should().BeFalse();
            phone.IsPrimary.Should().BeTrue(); // Original deve permanecer imutável
            secondaryPhone.AreaCode.Should().Be(phone.AreaCode);
            secondaryPhone.Number.Should().Be(phone.Number);
        }
    }
}