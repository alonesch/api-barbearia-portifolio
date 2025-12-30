using System.ComponentModel.DataAnnotations;

namespace BarbeariaPortifolio.API.Auth.DTOs
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(100)]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Senha { get; set; } = string.Empty;
    }
}
