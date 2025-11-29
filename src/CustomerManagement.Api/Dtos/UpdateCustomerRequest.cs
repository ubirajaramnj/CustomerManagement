using System.ComponentModel.DataAnnotations;

namespace CustomerManagement.Api.Dtos
{
    public class UpdateCustomerRequest
    {
        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}
