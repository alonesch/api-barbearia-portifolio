using BarbeariaPortifolio.API.Modules.Agendamentos.Models;
using BarbeariaPortifolio.API.Modules.Barbeiros.Models;

namespace BarbeariaPortifolio.API.Modules.Disponibilidades.Models;

public class Disponibilidade
{
    public int Id { get; set; }

    public int BarbeiroId { get; set; }

    public DateOnly Data { get; set; }

    public string Hora { get; set; } = null!;

    public bool Ativo { get; set; }

    public DateTime DataCriacao { get; set; }

    public Barbeiro Barbeiro { get; set; } = null!;

    
    public ICollection<Agendamento> Agendamentos { get; set; }
        = new List<Agendamento>();
}
