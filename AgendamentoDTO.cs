
namespace BarbeariaPortifolio.DTOs
{
    public class AgendamentoDTO
    {
        public int Id { get; set; }
        public string Cliente { get; set; }
        public string Barbeiro { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; }
        public string? Observacao { get; set; }
        public List<ServicoDTO> Servicos { get; set; }
    }

}