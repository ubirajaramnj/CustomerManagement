using CustomerManagement.Domain.Customers;
using CustomerManagement.Domain.Shared;
using FluentAssertions;

namespace CustomerManagement.Domain.Tests.Customers
{
    public class EmailTests
    {
        [Fact]
        public void Create_ValidEmail_ShouldReturnEmailObject()
        {
            // Arrange
            string emailValue = "teste@example.com";

            // Act
            var email = Email.Create(emailValue);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(emailValue);
            email.IsPrimary.Should().BeFalse();
        }

        [Fact]
        public void Create_ValidEmailAsPrimary_ShouldReturnPrimaryEmailObject()
        {
            // Arrange
            string emailValue = "principal@example.com";

            // Act
            var email = Email.Create(emailValue, true);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(emailValue);
            email.IsPrimary.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_NullOrEmptyEmail_ShouldThrowDomainException(string invalidEmail)
        {
            // Act
            Action act = () => Email.Create(invalidEmail);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O endereço de e-mail não pode ser vazio.");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("invalid@")]
        [InlineData("@domain.com")]
        [InlineData("user@domain")]
        public void Create_InvalidFormatEmail_ShouldThrowDomainException(string invalidEmail)
        {
            // Act
            Action act = () => Email.Create(invalidEmail);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O formato do e-mail '{invalidEmail}' é inválido.");
        }

        [Fact]
        public void Equals_TwoEmailsWithSameValue_ShouldReturnTrue()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("test@example.com");

            // Act & Assert
            email1.Should().Be(email2);
            (email1 == email2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoEmailsWithDifferentValue_ShouldReturnFalse()
        {
            // Arrange
            var email1 = Email.Create("test1@example.com");
            var email2 = Email.Create("test2@example.com");

            // Act & Assert
            email1.Should().NotBe(email2);
            (email1 != email2).Should().BeTrue();
        }

        [Fact]
        public void SetAsPrimary_ShouldReturnNewEmailWithIsPrimaryTrue()
        {
            // Arrange
            var email = Email.Create("test@example.com", false);

            // Act
            var primaryEmail = email.SetAsPrimary();

            // Assert
            primaryEmail.IsPrimary.Should().BeTrue();
            email.IsPrimary.Should().BeFalse(); // Original deve permanecer imutável
            primaryEmail.Value.Should().Be(email.Value);
        }

        [Fact]
        public void SetAsSecondary_ShouldReturnNewEmailWithIsPrimaryFalse()
        {
            // Arrange
            var email = Email.Create("test@example.com", true);

            // Act
            var secondaryEmail = email.SetAsSecondary();

            // Assert
            secondaryEmail.IsPrimary.Should().BeFalse();
            email.IsPrimary.Should().BeTrue(); // Original deve permanecer imutável
            secondaryEmail.Value.Should().Be(email.Value);
        }
    }
}