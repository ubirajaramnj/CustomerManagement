using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Api.Dtos
{
    public class DocumentRequest
    {
        [Required(ErrorMessage = "O tipo do documento é obrigatório.")]
        public string Type { get; set; } = string.Empty; // e.g., "CPF", "CNPJ"

        [Required(ErrorMessage = "O número do documento é obrigatório.")]
        public string Number { get; set; } = string.Empty;
    }

    public class DocumentResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
    }
}
