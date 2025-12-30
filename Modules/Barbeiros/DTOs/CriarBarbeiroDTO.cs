namespace BarbeariaPortifolio.API.Modules.Barbeiros.DTOs;

public class CriarBarbeiroDTO
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public int? UsuarioId { get; set; }
}
