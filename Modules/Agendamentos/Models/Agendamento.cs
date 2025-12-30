using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BarbeariaPortifolio.API.Modules.Agendamentos.Models.Enums;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using BarbeariaPortifolio.API.Modules.Barbeiros.Models;
using BarbeariaPortifolio.API.Modules.Disponibilidades.Models;

namespace BarbeariaPortifolio.API.Modules.Agendamentos.Models;

[Table("Agendamento")]
public class Agendamento
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Usuario")]
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    [ForeignKey("Barbeiro")]
    public int BarbeiroId { get; set; }
    public Barbeiro Barbeiro { get; set; } = null!;

   
    [ForeignKey("Disponibilidade")]
    public int? DisponibilidadeId { get; set; }

    
    public Disponibilidade Disponibilidade { get; set; } = null!;

    [Column(TypeName = "timestamp with time zone")]
    public DateTime DataHora { get; set; }

    public StatusAgendamento Status {  get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "varchar(255)")]
    public string? Observacao { get; set; }

    public ICollection<AgendamentoServico> AgendamentoServicos { get; set; }
        = new List<AgendamentoServico>();
}
