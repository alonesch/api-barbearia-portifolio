using BarbeariaPortifolio.API.Modules.Agendamentos.Models;
using BarbeariaPortifolio.API.Modules.Services.Models;

public class AgendamentoServico
{
    public int AgendamentoId { get; set; }
    public int ServicoId { get; set; }

    public string? Observacao { get; set; }

    public Servico Servico { get; set; } = null!;
    public Agendamento Agendamento { get; set; } = null!;
}
