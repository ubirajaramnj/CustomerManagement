using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Api.Dtos
{
    public class PhoneRequest
    {
        [Required(ErrorMessage = "O código do país é obrigatório.")]
        [StringLength(5, ErrorMessage = "O código do país não pode exceder 5 caracteres.")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número do telefone é obrigatório.")]
        [StringLength(15, ErrorMessage = "O número do telefone não pode exceder 15 caracteres.")]
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

    public class PhoneResponse
    {
        public string CountryCode { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}
