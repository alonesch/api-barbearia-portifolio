namespace BarbeariaPortifolio.API.Modules.Disponibilidades.DTOs;

public class DisponibilidadeResponseDto
{
    public int Id { get; set; }
    public DateOnly Data { get; set; }
    public string Hora { get; set; } = null!;
    public bool Ativo { get; set; }
}
