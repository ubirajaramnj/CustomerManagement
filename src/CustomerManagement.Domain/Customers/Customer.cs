using CustomerManagement.Domain.Shared;

namespace CustomerManagement.Domain.Customers
{
    public class Customer : Entity
    {
        public string Name { get; private set; }

        private readonly List<Email> _emails;
        public IReadOnlyList<Email> Emails => _emails.AsReadOnly();
        
        private readonly List<Phone> _phones;
        public IReadOnlyList<Phone> Phones => _phones.AsReadOnly();
        
        private readonly List<Address> _addresses;
        public IReadOnlyList<Address> Addresses => _addresses.AsReadOnly();
        
        private readonly List<Document> _documents;
        public IReadOnlyList<Document> Documents => _documents.AsReadOnly();
        
        public DateTime CreatedAt { get; private set; }
        
        public DateTime UpdatedAt { get; private set; }
        
        public bool IsActive { get; private set; }

        // Construtor privado para forçar o uso do Factory Method
        private Customer(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                throw new DomainException("O nome do cliente deve ter pelo menos 3 caracteres.");
            }

            Name = name;
            _emails = [];
            _phones = [];
            _addresses = [];
            _documents = [];
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public static Customer Create(string name)
        {
            return new Customer(name);
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length < 3)
                throw new DomainException("O nome do cliente deve ter pelo menos 3 caracteres.");
            
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddEmail(string value, bool isPrimary = false)
        {
            if (_emails.Any(e => e.Value.Equals(value, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException($"O e-mail '{value}' já existe para este cliente.");
            
            var email = Email.Create(value, isPrimary);
            _emails.Add(email);

            if (_emails.Count > 0 && email.IsPrimary)
                SetPrimaryEmail(email.Value); // Garante que apenas um seja primário

            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveEmail(string emailValue)
        {
            var emailToRemove = _emails.FirstOrDefault(e => e.Value.Equals(emailValue, StringComparison.OrdinalIgnoreCase));
            if (emailToRemove == null)
                throw new DomainException($"E-mail '{emailValue}' não encontrado para remoção.");
            
            if (_emails.Count == 1)
                throw new DomainException("Não é possível remover o último e-mail do cliente.");

            _emails.Remove(emailToRemove);
            
            if (emailToRemove.IsPrimary && _emails.Any())
                _emails.First().SetAsPrimary(); // Define o primeiro como primário se o removido era
            
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPrimaryEmail(string emailValue)
        {
            var targetEmail = _emails.FirstOrDefault(e => e.Value.Equals(emailValue, StringComparison.OrdinalIgnoreCase));
            if (targetEmail == null)
                throw new DomainException($"E-mail '{emailValue}' não encontrado para ser definido como principal.");

            for (int i = 0; i < _emails.Count; i++)
            {
                if (_emails[i].Value.Equals(emailValue, StringComparison.OrdinalIgnoreCase))
                {
                    _emails[i] = _emails[i].SetAsPrimary();
                }
                else if (_emails[i].IsPrimary)
                {
                    _emails[i] = _emails[i].SetAsSecondary();
                }
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void AddPhone(Phone phone)
        {
            if (_phones.Any(p => p.AreaCode == phone.AreaCode && p.Number == phone.Number))
                throw new DomainException($"O telefone '({phone.AreaCode}) {phone.Number}' já existe para este cliente.");

            if (_phones.Count > 0 && phone.IsPrimary)
                SetPrimaryPhone(phone.AreaCode, phone.Number); // Garante que apenas um seja primário

            _phones.Add(phone);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemovePhone(string areaCode, string number)
        {
            var phoneToRemove = _phones.FirstOrDefault(p => p.AreaCode == areaCode && p.Number == number);
            if (phoneToRemove == null)
                throw new DomainException($"Telefone '({areaCode}) {number}' não encontrado para remoção.");

            if (_phones.Count == 1)
                throw new DomainException("Não é possível remover o último telefone do cliente.");

            _phones.Remove(phoneToRemove);

            if (phoneToRemove.IsPrimary && _phones.Any())
                _phones.First().SetAsPrimary(); // Define o primeiro como primário se o removido era

            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPrimaryPhone(string areaCode, string number)
        {
            var targetPhone = _phones.FirstOrDefault(p => p.AreaCode == areaCode && p.Number == number);
            if (targetPhone == null)
                throw new DomainException($"Telefone '({areaCode}) {number}' não encontrado para ser definido como principal.");

            if (targetPhone.IsPrimary) return; // Já é primário

            for (int i = 0; i < _phones.Count; i++)
            {
                if (_phones[i].AreaCode == areaCode && _phones[i].Number == number)
                {
                    _phones[i] = _phones[i].SetAsPrimary();
                }
                else if (_phones[i].IsPrimary)
                {
                    _phones[i] = _phones[i].SetAsSecondary();
                }
            }
            
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAddress(Address address)
        {
            if (_addresses.Any(a => a.Equals(address)))
                throw new DomainException($"O endereço '{address}' já existe para este cliente.");

            if (_addresses.Count > 0 && address.IsPrimary)
                SetPrimaryAddress(address.Street, address.Number, address.City); // Garante que apenas um seja primário
            
            _addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveAddress(string street, string number, string city)
        {
            var addressToRemove = _addresses.FirstOrDefault(a => a.Street.Equals(street, StringComparison.OrdinalIgnoreCase) &&
                                                                a.Number.Equals(number, StringComparison.OrdinalIgnoreCase) &&
                                                                a.City.Equals(city, StringComparison.OrdinalIgnoreCase));
            if (addressToRemove == null)
                throw new DomainException($"Endereço '{street}, {number} - {city}' não encontrado para remoção.");
            
            if (_addresses.Count == 1)
                throw new DomainException("Não é possível remover o último endereço do cliente.");
            
            _addresses.Remove(addressToRemove);
            
            if (addressToRemove.IsPrimary && _addresses.Any())
                _addresses.First().SetAsPrimary(); // Define o primeiro como primário se o removido era
            
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPrimaryAddress(string street, string number, string city)
        {
            var targetAddress = _addresses.FirstOrDefault(a => a.Street.Equals(street, StringComparison.OrdinalIgnoreCase) &&
                                                              a.Number.Equals(number, StringComparison.OrdinalIgnoreCase) &&
                                                              a.City.Equals(city, StringComparison.OrdinalIgnoreCase));
            if (targetAddress == null)
                throw new DomainException($"Endereço '{street}, {number} - {city}' não encontrado para ser definido como principal.");

            if (targetAddress.IsPrimary) return; // Já é primário

            for (int i = 0; i < _addresses.Count; i++)
            {
                if (_addresses[i].Street.Equals(street, StringComparison.OrdinalIgnoreCase) &&
                    _addresses[i].Number.Equals(number, StringComparison.OrdinalIgnoreCase) &&
                    _addresses[i].City.Equals(city, StringComparison.OrdinalIgnoreCase))
                {
                    _addresses[i] = _addresses[i].SetAsPrimary();
                }
                else if (_addresses[i].IsPrimary)
                {
                    _addresses[i] = _addresses[i].SetAsSecondary();
                }
            }
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddDocument(Document document)
        {
            if (_documents.Any(d => d.Number == document.Number && d.Type == document.Type))
                throw new DomainException($"O documento '{document.Number}' do tipo '{document.Type}' já existe para este cliente.");

            _documents.Add(document);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveDocument(string documentNumber)
        {
            var documentToRemove = _documents.FirstOrDefault(d => d.Number == documentNumber);
            if (documentToRemove == null)
                throw new DomainException($"Documento '{documentNumber}' não encontrado para remoção.");
            
            _documents.Remove(documentToRemove);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("O cliente já está inativo.");
            
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("O cliente já está ativo.");
            
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        // Validações pós-construção ou antes de persistir
        public void ValidateCustomerState()
        {
            if (!_emails.Any())
                throw new DomainException("O cliente deve ter pelo menos um e-mail.");
            
            if (!_emails.Any(e => e.IsPrimary))
                throw new DomainException("O cliente deve ter um e-mail principal.");
            
            if (!_phones.Any())
                throw new DomainException("O cliente deve ter pelo menos um telefone.");
            
            if (!_phones.Any(p => p.IsPrimary))
                throw new DomainException("O cliente deve ter um telefone principal.");

            if (!_addresses.Any())
                throw new DomainException("O cliente deve ter pelo menos um endereço.");
            
            if (!_addresses.Any(a => a.IsPrimary))
                throw new DomainException("O cliente deve ter um endereço principal.");

            if (!_documents.Any())
                throw new DomainException("O cliente deve ter pelo menos um documento.");
        }
    }
}