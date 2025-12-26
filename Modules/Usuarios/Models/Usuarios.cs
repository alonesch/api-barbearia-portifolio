using System.ComponentModel.DataAnnotations;
using BarbeariaPortifolio.API.Modules.Barbeiros.Models;

namespace BarbeariaPortifolio.API.Modules.Usuarios.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    public string NomeUsuario { get; set; } = string.Empty;   

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;         

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    [Required]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required]
    public string Cargo { get; set; } = string.Empty;         

    public bool EmailConfirmado { get; set; } = false;

    public bool Ativo { get; set; } = true;

    public string? FotoPerfilUrl { get; set; }

    public Barbeiro? Barbeiro { get; set; }
}
