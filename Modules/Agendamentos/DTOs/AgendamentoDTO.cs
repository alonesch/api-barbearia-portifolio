

namespace BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;

public class AgendamentoDTO
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Email { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public int BarbeiroId { get; set; }
    public DateTime DataHora { get; set; }
    public int Status { get; set; }
    public string? Observacao { get; set; }
    public List<AgendamentoServicoDTO> AgendamentoServicos{ get; set; } = new();
}