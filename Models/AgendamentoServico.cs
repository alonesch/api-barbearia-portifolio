using BarbeariaPortifolio.API.Models;

public class AgendamentoServico
{
    public int AgendamentoId { get; set; }
    public int ServicoId { get; set; }
    public string? Observacao { get; set; }

    public Servico Servico { get; set; }
    public Agendamento Agendamento { get; set; }
}
