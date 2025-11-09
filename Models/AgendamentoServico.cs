using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarbeariaPortifolio.API.Models;

[Table("AgendamentoServico")]
public class AgendamentoServico
{
    [Key, Column(Order = 0)]
    [ForeignKey("Agendamento")]
    public int AgendamentoId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Servico")]
    public int ServicoId { get; set; }

    public int Quantidade { get; set; } = 1;

    [Column(TypeName = "varchar(255)")]
    public string? Observacao { get; set; }

    public Agendamento? Agendamento { get; set; }
    public Servico? Servico { get; set; }
}
