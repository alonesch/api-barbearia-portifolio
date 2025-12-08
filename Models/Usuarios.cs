using System.ComponentModel.DataAnnotations;

namespace BarbeariaPortifolio.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string NomeUsuario { get; set; } = string.Empty;   // atalho de login

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;         // identificador principal

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        public string Cargo { get; set; } = string.Empty;         // Admin | Barbeiro | Cliente

        public bool EmailConfirmado { get; set; } = false;

        public bool Ativo { get; set; } = true;
    }
}
