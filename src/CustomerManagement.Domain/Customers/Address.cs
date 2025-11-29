using CustomerManagement.Domain.Shared;

namespace CustomerManagement.Domain.Customers
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string Number { get; private set; }
        public string Complement { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public string Country { get; private set; }
        public bool IsPrimary { get; private set; }

        // Construtor privado para forçar o uso do Factory Method
        private Address(string street, string number, string complement, string city, string state, string zipCode, string country, bool isPrimary)
        {
            Street = street;
            Number = number;
            Complement = complement;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            IsPrimary = isPrimary;
        }

        public static Address Create(string street, string number, string complement, string city, string state, string zipCode, string country, bool isPrimary = false)
        {
            if (string.IsNullOrWhiteSpace(street)) throw new DomainException("A rua não pode ser vazia.");
            if (string.IsNullOrWhiteSpace(number)) throw new DomainException("O número do endereço não pode ser vazio.");
            if (string.IsNullOrWhiteSpace(city)) throw new DomainException("A cidade não pode ser vazia.");
            if (string.IsNullOrWhiteSpace(state)) throw new DomainException("O estado não pode ser vazio.");
            if (string.IsNullOrWhiteSpace(zipCode)) throw new DomainException("O CEP não pode ser vazio.");
            if (string.IsNullOrWhiteSpace(country)) throw new DomainException("O país não pode ser vazio.");

            return new Address(street, number, complement, city, state, zipCode, country, isPrimary);
        }

        public Address SetAsPrimary() => new(Street, Number, Complement, City, State, ZipCode, Country, true);

        public Address SetAsSecondary() => new(Street, Number, Complement, City, State, ZipCode, Country, false);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return Number;
            yield return City;
            yield return State;
            yield return ZipCode;
            yield return Country;
        }

        public override string ToString() => $"{Street}, {Number} - {City}/{State}";
    }
}