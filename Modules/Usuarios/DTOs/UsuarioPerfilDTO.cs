namespace BarbeariaPortifolio.API.Modules.Usuarios.DTOs;

public class UsuarioPerfilDTO
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public string? FotoPerfilUrl { get; set; }
    public int? BarbeiroId { get; set; }
}
