namespace BarbeariaPortifolio.DTOs;

public class ServicoDTO
{
    public required string NomeServico { get; set; }
    public string? Observacao { get; set; }

    public decimal Preco { get; set; }
    public AgendamentoDTO? Agendamento { get; set; }

    
}
