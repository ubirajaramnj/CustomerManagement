using CustomerManagement.Domain.Shared;
using System.Text.RegularExpressions;

namespace CustomerManagement.Domain.Customers
{
    public class Email : ValueObject
    {
        public string Value { get; private set; }
        public bool IsPrimary { get; private set; }

        // Construtor privado para forçar o uso do Factory Method
        private Email(string value, bool isPrimary)
        {
            Value = value;
            IsPrimary = isPrimary;
        }

        public static Email Create(string email, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException("O endereço de e-mail não pode ser vazio.");
            }

            // Regex para validação de e-mail (simplificado para exemplo)
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                throw new DomainException($"O formato do e-mail '{email}' é inválido.");
            }

            return new Email(email, isPrimary);
        }

        public Email SetAsPrimary()
        {
            return new Email(Value, true);
        }

        public Email SetAsSecondary()
        {
            return new Email(Value, false);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}