namespace BarbeariaPortifolio.API.Modules.Usuarios.DTOs;

public class UsuarioDTO
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public int? BarbeiroId { get; set; }
    public string? FotoPerfilUrl { get; set; }
}