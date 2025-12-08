using BarbeariaPortifolio.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Cliente")] // ou "Clientes", se quiser padronizar
public class Cliente
{
    [Key]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    [Column(TypeName = "varchar(15)")]
    public string? Cpf { get; set; }

    [Required]
    [Column(TypeName = "varchar(15)")]
    public string Telefone { get; set; } = null!;

    [Column(TypeName = "timestamp with time zone")]
    public DateTime DataCadastro { get; set; }
}
