

public class ServicoDTO
{
    public required string NomeServico { get; set; }
    public decimal Preco { get; set; }
}

public class AgendamentoDTO
{
    public int Id { get; set; }
    public required string Cliente { get; set; }
    public required string Barbeiro { get; set; }
    public DateTime DataHora { get; set; }
    public required string Status { get; set; }
    public string? Observacao { get; set; }
    public List<ServicoDTO> Servicos { get; set; } = new(); // inicializado
}
