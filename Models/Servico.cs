using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarbeariaPortifolio.API.Models;

[Table("Servico")]
public class Servico
{
    [Key]
    public int ID { get; set; }

    [Column(TypeName = "varchar(100)")]
    [Required]
    public string NomeServico { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    [Required]
    public decimal Preco { get; set; }

    public ICollection<AgendamentoServico>? AgendamentoServicos { get; set; }
}
