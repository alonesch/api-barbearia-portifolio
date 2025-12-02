using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using BarbeariaPortifolio.API.Models;

[Table("Cliente")]
public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "varchar(150)")]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column(TypeName = "varchar(15)")]
    [Required]
    public string? Cpf { get; set; }

    [Column(TypeName = "varchar(15)")]
    [Required]
    public string Telefone { get; set; } = string.Empty;

    [Column(TypeName = "timestamp with time zone")]
    public DateTime DataCadastro { get; set; }

    public virtual ICollection<Agendamento>? Agendamentos { get; set; }
}
