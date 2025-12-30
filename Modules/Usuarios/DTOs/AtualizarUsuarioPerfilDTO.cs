using System.ComponentModel.DataAnnotations;

namespace BarbeariaPortifolio.API.Modules.Usuarios.DTOs;

public class AtualizarUsuarioPerfilDTO
{
    [MinLength(3)]
    public string? NomeCompleto { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? Telefone { get; set; }
}
