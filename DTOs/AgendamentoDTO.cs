namespace BarbeariaPortifolio.DTOs
{
    public class AgendamentoDTO
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public int BarbeiroId { get; set; }

        public DateTime DataHora { get; set; }
        public int Status { get; set; }
        public string? Observacao { get; set; }

        public List<int> ServicosIds { get; set; } = new();
    }
}
