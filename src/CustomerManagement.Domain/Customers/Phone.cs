using CustomerManagement.Domain.Shared;

namespace CustomerManagement.Domain.Customers
{
    public class Phone : ValueObject
    {
        public string AreaCode { get; private set; }
        public string Number { get; private set; }
        public bool IsPrimary { get; private set; }

        // Construtor privado para forçar o uso do Factory Method
        private Phone(string areaCode, string number, bool isPrimary)
        {
            AreaCode = areaCode;
            Number = number;
            IsPrimary = isPrimary;
        }

        public static Phone Create(string areaCode, string number, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(areaCode))
            {
                throw new DomainException("O código de área não pode ser vazio.");
            }
            if (areaCode.Length != 2 || !areaCode.All(char.IsDigit))
            {
                throw new DomainException($"O código de área '{areaCode}' deve ter 2 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(number))
            {
                throw new DomainException("O número de telefone não pode ser vazio.");
            }
            if (number.Length < 8 || number.Length > 9 || !number.All(char.IsDigit))
            {
                throw new DomainException($"O número de telefone '{number}' deve ter entre 8 e 9 dígitos numéricos.");
            }

            return new Phone(areaCode, number, isPrimary);
        }

        public Phone SetAsPrimary()
        {
            return new Phone(AreaCode, Number, true);
        }

        public Phone SetAsSecondary()
        {
            return new Phone(AreaCode, Number, false);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return AreaCode;
            yield return Number;
        }
        public override string ToString() => $"({AreaCode}) {Number}";
    }
}