namespace BarbeariaPortifolio.API.Modules.Disponibilidades.DTOs;

public class CriarDisponibilidadeDto
{
    public DateOnly Data { get; set; }
    public string Hora { get; set; } = string.Empty; // "09:00"
}
