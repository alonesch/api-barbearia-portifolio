using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarbeariaPortifolio.API.Modules.Clientes.Models;

[Table("Cliente")] 
public class Cliente
{
    [Key]
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    [Column(TypeName = "varchar(15)")]
    public string? Cpf { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTime DataCadastro { get; set; }
}
