using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.DTOs.Disponibilidade;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Exceptions;
using Microsoft.EntityFrameworkCore;

public class DisponibilidadeServico : IDisponibilidadeServico
{
    private readonly DataContext _repositorio;

    public DisponibilidadeServico(DataContext repositorio)
    {
        _repositorio = repositorio;
    }

    // ✅ CRIAR OU REATIVAR SLOT
    public async Task CriarDisponibilidadeAsync(int barbeiroId, CriarDisponibilidadeDto dto)
    {
        if (dto.Data < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new AppException("Ainda não inventaram máquina do tempo!", 405);

        if (string.IsNullOrWhiteSpace(dto.Hora))
            throw new AppException("Hora é obrigatória", 400);

        // ✅ VERIFICA SE JÁ EXISTE O SLOT
        var existente = await _repositorio.Disponibilidades.FirstOrDefaultAsync(x =>
            x.BarbeiroId == barbeiroId &&
            x.Data == dto.Data &&
            x.Hora == dto.Hora
        );

        // ✅ SE EXISTE → REATIVA
        if (existente != null)
        {
            existente.Ativo = true;
            await _repositorio.SaveChangesAsync();
            return;
        }

        // ✅ SE NÃO EXISTE → CRIA NOVO
        var disponibilidade = new Disponibilidade
        {
            BarbeiroId = barbeiroId,
            Data = dto.Data,
            Hora = dto.Hora,
            Ativo = true,
            DataCriacao = DateTime.UtcNow
        };

        _repositorio.Disponibilidades.Add(disponibilidade);
        await _repositorio.SaveChangesAsync();
    }

    // ✅ LISTAR DISPONIBILIDADES PÚBLICAS (CLIENTE)
    public async Task<IEnumerable<DisponibilidadeResponseDto>> ListarDisponibilidadesPublicasAsync(int barbeiroId, DateOnly data)
    {
        return await _repositorio.Disponibilidades
            .Where(x =>
                x.BarbeiroId == barbeiroId &&
                x.Ativo &&
                x.Data == data
            )
            .OrderBy(x => x.Hora)
            .Select(x => new DisponibilidadeResponseDto
            {
                Id = x.Id,
                Data = x.Data,
                Hora = x.Hora,
                Ativo = x.Ativo
            })
            .ToListAsync();
    }

    // ✅ LISTAR TODAS AS DISPONIBILIDADES DO BARBEIRO (ATIVAS + INATIVAS)
    public async Task<IEnumerable<DisponibilidadeResponseDto>> ListarDisponibilidadesDoBarbeiroAsync(int barbeiroId, DateOnly data)
    {
        return await _repositorio.Disponibilidades
            .Where(x =>
                x.BarbeiroId == barbeiroId &&
                x.Data == data
            )
            .OrderBy(x => x.Hora)
            .Select(x => new DisponibilidadeResponseDto
            {
                Id = x.Id,
                Data = x.Data,
                Hora = x.Hora,
                Ativo = x.Ativo
            })
            .ToListAsync();
    }

    // ✅ ATIVAR / INATIVAR SLOT
    public async Task AtualizarStatusAsync(int disponibilidadeId, bool ativo, int barbeiroId)
    {
        var disponibilidade = await _repositorio.Disponibilidades
            .FirstOrDefaultAsync(x =>
                x.Id == disponibilidadeId &&
                x.BarbeiroId == barbeiroId
            );

        if (disponibilidade == null)
            throw new AppException("Disponibilidade não encontrada", 404);

        disponibilidade.Ativo = ativo;
        await _repositorio.SaveChangesAsync();
    }
}
