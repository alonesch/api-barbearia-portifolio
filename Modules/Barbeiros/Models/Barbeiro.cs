using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BarbeariaPortifolio.API.Modules.Agendamentos.Models;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;

namespace BarbeariaPortifolio.API.Modules.Barbeiros.Models;

[Table("Barbeiro")]
public class Barbeiro
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "varchar(150)")]
    public string? Nome { get; set; }

    [Column(TypeName = "varchar(15)")]
    [Required]
    public string Telefone { get; set; } = string.Empty;

    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<Agendamento>? Agendamentos { get; set; }
}
