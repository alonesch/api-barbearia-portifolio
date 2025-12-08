namespace BarbeariaPortifolio.API.DTOs
{
    public class AgendamentoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int BarbeiroId { get; set; }

        public DateTime DataHora { get; set; }

        public int Status { get; set; } = 1;

        public string? Observacao { get; set; }

        public List<AgendamentoServicoDTO> AgendamentoServicos { get; set; } = new();
    }

    public class AgendamentoServicoDTO
    {
        public int ServicoId { get; set; }
        public string? Observacao { get; set; }
    }
}
