using BarbeariaPortifolio.API.Modules.Disponibilidades.DTOs;

namespace BarbeariaPortifolio.API.Modules.Disponibilidades.Services.Interfaces;

public interface IDisponibilidadeServico
{
    Task CriarDisponibilidadeAsync(int barbeiroId, CriarDisponibilidadeDto dto);

    Task<IEnumerable<DisponibilidadeResponseDto>> ListarDisponibilidadesPublicasAsync(
        int barbeiroId,
        DateOnly data
    );

    Task<IEnumerable<DisponibilidadeResponseDto>> ListarDisponibilidadesDoBarbeiroAsync(
        int barbeiroId,
        DateOnly data
    );

    Task AtualizarStatusAsync(int disponibilidadeId, bool ativo, int barbeiroId);

    Task<bool> ReservarSlotAsync(int barbeiroId, DateOnly data, string hora);
    Task<bool> LiberarSlotAsync(int barbeiroId, DateOnly data, string hora);
}
