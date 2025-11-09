using BarbeariaPortifolio.API.Models;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Agendamento")]
public class Agendamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public int BarbeiroId { get; set; }
    public Barbeiro Barbeiro { get; set; } = null!;
    public DateTime DataHora { get; set; }
    public int Status { get; set; }
    public DateTime DataRegistro { get; set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; set; } = new List<AgendamentoServico>();
}
