using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarbeariaPortifolio.API.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    public int ID { get; set; }

    [Column(TypeName = "varchar(150)")]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column(TypeName = "varchar(15)")]
    public string? Cpf { get; set; }

    [Column(TypeName = "varchar(15)")]
    [Required]
    public string Telefone { get; set; } = string.Empty;

    [Column(TypeName = "datetime")]
    public DateTime? DataCadastro { get; set; } = DateTime.Now;

    public ICollection<Agendamento>? Agendamentos { get; set; }
}
