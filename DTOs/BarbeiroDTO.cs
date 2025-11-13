namespace BarbeariaPortifolio.DTOs
{

    public class BarbeiroDTO
    {
        public int Id { get; set; }
        public string? Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Usuario { get; set; }
        public List<AgendamentoDTO>? Agendamentos { get; set; }

    }
}