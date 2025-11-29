using System.ComponentModel.DataAnnotations;
using static CustomerManagement.API.Controllers.CustomersController;

namespace CustomerManagement.Api.Dtos
{
    public class CreateCustomerRequest
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Um email primário é obrigatório.")]
        public EmailRequest PrimaryEmail { get; set; } = new EmailRequest { IsPrimary = true };

        [Required(ErrorMessage = "Um telefone primário é obrigatório.")]
        public PhoneRequest PrimaryPhone { get; set; } = new PhoneRequest { IsPrimary = true };

        [Required(ErrorMessage = "Um endereço primário é obrigatório.")]
        public AddressRequest PrimaryAddress { get; set; } = new AddressRequest { IsPrimary = true };

        public List<DocumentRequest> Documents { get; set; } = new List<DocumentRequest>();
    }

    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<EmailResponse> Emails { get; set; } = new List<EmailResponse>();
        public List<PhoneResponse> Phones { get; set; } = new List<PhoneResponse>();
        public List<AddressResponse> Addresses { get; set; } = new List<AddressResponse>();
        public List<DocumentResponse> Documents { get; set; } = new List<DocumentResponse>();
    }
    
}
