using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Api.Dtos
{
    public class EmailRequest
    {
        [Required(ErrorMessage = "O valor do email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }
    public class EmailResponse
    {
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

}
