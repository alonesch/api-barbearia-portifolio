namespace BarbeariaPortfolio.API.Models
{
    public class AgendamentoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public int BarbeiroId { get; set; }
        public int ServicoId { get; set; }
        public DateTime DataHora { get; set; }
    }
}
