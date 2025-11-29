using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Api.Dtos
{
    public class AddressRequest
    {
        [Required(ErrorMessage = "A rua é obrigatória.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número do endereço é obrigatório.")]
        public string Number { get; set; } = string.Empty;

        public string? Complement { get; set; }

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado é obrigatório.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "O país é obrigatório.")]
        public string Country { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

    public class AddressResponse
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Complement { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
