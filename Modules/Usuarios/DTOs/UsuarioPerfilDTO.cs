namespace BarbeariaPortifolio.API.Modules.Usuarios.DTOs;

public class UsuarioPerfilDTO
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string? FotoPerfilUrl { get; set; }
}
