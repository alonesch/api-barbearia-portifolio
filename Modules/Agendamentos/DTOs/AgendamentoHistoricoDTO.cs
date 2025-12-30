namespace BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;

public class AgendamentoHistoricoDTO
{
    public int Id { get; set; }
    public string ClienteNome { get; set; } = null!;
    public DateTime DataHora { get; set; }
    public string Status { get; set; } = null!;
    public decimal ValorTotal { get; set; }
    public List<ServicoResumoDTO> Servicos { get; set; } = new();
}

public class ServicoResumoDTO
{
    public int Id { get; set; }
    public string NomeServico { get; set; } = null!;
    public decimal Preco { get; set; }
}
