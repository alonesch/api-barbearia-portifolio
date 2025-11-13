namespace BarbeariaPortifolio.DTOs;

public class AgendamentoDTO
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Barbeiro { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public int Status { get; set; }
    public string? Observacao { get; set; }
    public List<ServicoDTO> Servicos { get; set; } = new();

}
