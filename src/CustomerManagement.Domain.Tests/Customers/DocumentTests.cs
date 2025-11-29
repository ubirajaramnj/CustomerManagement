using CustomerManagement.Domain.Shared;
using CustomerManagement.Domain.Customers;
using FluentAssertions;

namespace CustomerManagement.Domain.Tests.Customers
{
    public class DocumentTests
    {
        [Fact]
        public void Create_ValidCpf_ShouldReturnDocumentObject()
        {
            // Arrange
            string cpfNumber = "12345678901"; // CPF válido simplificado

            // Act
            var document = Document.Create(cpfNumber, "CPF");

            // Assert
            document.Should().NotBeNull();
            document.Number.Should().Be(cpfNumber);
            document.Type.Should().Be("CPF");
        }

        [Fact]
        public void Create_ValidCnpj_ShouldReturnDocumentObject()
        {
            // Arrange
            string cnpjNumber = "12345678000100"; // CNPJ válido simplificado

            // Act
            var document = Document.Create(cnpjNumber, "CNPJ");

            // Assert
            document.Should().NotBeNull();
            document.Number.Should().Be(cnpjNumber);
            document.Type.Should().Be("CNPJ");
        }

        [Theory]
        [InlineData(null, "CPF")]
        [InlineData("", "CPF")]
        [InlineData(" ", "CPF")]
        public void Create_NullOrEmptyDocumentNumber_ShouldThrowDomainException(string invalidNumber, string type)
        {
            // Act
            Action act = () => Document.Create(invalidNumber, type);

            // Assert
            act.Should().Throw<DomainException>().WithMessage("O número do documento não pode ser vazio.");
        }

        [Theory]
        [InlineData("123", "CPF")] // CPF com menos de 11 dígitos
        [InlineData("123456789012", "CPF")] // CPF com mais de 11 dígitos
        [InlineData("ABCDEFGHIJK", "CPF")] // CPF não numérico
        public void Create_InvalidCpfFormat_ShouldThrowDomainException(string invalidCpf, string type)
        {
            // Act
            Action act = () => Document.Create(invalidCpf, type);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O CPF '{invalidCpf}' é inválido.");
        }

        [Theory]
        [InlineData("1234567800010", "CNPJ")] // CNPJ com menos de 14 dígitos
        [InlineData("123456780001000", "CNPJ")] // CNPJ com mais de 14 dígitos
        [InlineData("ABCDEFGHIJKLMNOP", "CNPJ")] // CNPJ não numérico
        public void Create_InvalidCnpjFormat_ShouldThrowDomainException(string invalidCnpj, string type)
        {
            // Act
            Action act = () => Document.Create(invalidCnpj, type);

            // Assert
            act.Should().Throw<DomainException>().WithMessage($"O CNPJ '{invalidCnpj}' é inválido.");
        }

        [Fact]
        public void Equals_TwoDocumentsWithSameNumberAndType_ShouldReturnTrue()
        {
            // Arrange
            var doc1 = Document.Create("11122233344", "CPF");
            var doc2 = Document.Create("11122233344", "CPF");

            // Act & Assert
            doc1.Should().Be(doc2);
            (doc1 == doc2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoDocumentsWithDifferentNumber_ShouldReturnFalse()
        {
            // Arrange
            var doc1 = Document.Create("11122233344", "CPF");
            var doc2 = Document.Create("55566677788", "CPF");

            // Act & Assert
            doc1.Should().NotBe(doc2);
            (doc1 != doc2).Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoDocumentsWithSameNumberButDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var doc1 = Document.Create("12345678901", "CNH");
            var doc2 = Document.Create("12345678901", "ID");

            // Act & Assert
            doc1.Should().NotBe(doc2);
            (doc1 != doc2).Should().BeTrue();
        }
    }
}