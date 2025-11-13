namespace BarbeariaPortifolio.DTOs
{
    public class ServicoDTO
    {
        public int Id { get; set; }
        public string NomeServico { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? Observacao { get; set; }
    }
}
