using CustomerManagement.Domain.Shared;
using System.Xml.Linq;

namespace CustomerManagement.Domain.Customers
{
    
    public class Document : ValueObject
    {
        public string Number { get; private set; }
        public string Type { get; private set; }

        // Construtor privado para forçar o uso do Factory Method
        private Document(string number, string type)
        {
            Number = number;
            Type = type;
        }

        public static Document Create(string number, string type)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new DomainException("O número do documento não pode ser vazio.");
            }

            string cleanedNumber = new string(number.Where(char.IsDigit).ToArray());

            switch (type)
            {
                case "CPF":
                    if (!IsValidCpf(cleanedNumber))
                    {
                        throw new DomainException($"O CPF '{number}' é inválido.");
                    }
                    break;
                case "CNPJ":
                    if (!IsValidCnpj(cleanedNumber))
                    {
                        throw new DomainException($"O CNPJ '{number}' é inválido.");
                    }
                    break;
            }

            return new Document(cleanedNumber, type);
        }

        private static bool IsValidCpf(string cpf)
        {
            if (cpf.Length != 11 || !cpf.All(char.IsDigit)) return false;
            // Implementação simplificada de validação de CPF (apenas para exemplo)
            // Uma validação real de CPF é mais complexa
            return true;
        }

        private static bool IsValidCnpj(string cnpj)
        {
            if (cnpj.Length != 14 || !cnpj.All(char.IsDigit)) return false;
            // Implementação simplificada de validação de CNPJ (apenas para exemplo)
            // Uma validação real de CNPJ é mais complexa
            return true;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return Type;
        }

        public override string ToString() => $"{Type}: {Number}";
    }
}