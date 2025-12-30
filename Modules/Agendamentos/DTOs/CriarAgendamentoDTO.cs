
namespace BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;

public class CriarAgendamentoDTO
{
    public int DisponibilidadeId { get; set; }
    public string? Observacao { get; set; }
    public List<AgendamentoServicoDTO> AgendamentoServicos { get; set; } = new();
}
