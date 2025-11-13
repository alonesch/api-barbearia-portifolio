using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarbeariaPortifolio.API.Models;

[Table("Agendamento")]
public class Agendamento
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Cliente")]
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    [ForeignKey("Barbeiro")]
    public int BarbeiroId { get; set; }
    public Barbeiro Barbeiro { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DataHora { get; set; }

    public int Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DataRegistro { get; set; } = DateTime.Now;

    public ICollection<AgendamentoServico> AgendamentoServicos { get; set; } = new List<AgendamentoServico>();
}
