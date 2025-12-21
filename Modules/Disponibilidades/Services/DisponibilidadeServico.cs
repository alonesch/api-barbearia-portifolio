using BarbeariaPortifolio.API.Shared.Data;
using BarbeariaPortifolio.API.Modules.Disponibilidades.Models;
using BarbeariaPortifolio.API.Modules.Disponibilidades.DTOs;
using BarbeariaPortifolio.API.Modules.Disponibilidades.Services.Interfaces;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Agendamentos.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Modules.Disponibilidades.Services;

public class DisponibilidadeServico : IDisponibilidadeServico
{
    private readonly DataContext _repositorio;

    public DisponibilidadeServico(DataContext repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task CriarDisponibilidadeAsync(int barbeiroId, CriarDisponibilidadeDto dto)
    {
        if (dto.Data < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new AppException("Ainda não inventaram máquina do tempo!", 405);

        if (string.IsNullOrWhiteSpace(dto.Hora))
            throw new AppException("Hora é obrigatória", 400);

        var existente = await _repositorio.Disponibilidades
            .FirstOrDefaultAsync(x =>
                x.BarbeiroId == barbeiroId &&
                x.Data == dto.Data &&
                x.Hora == dto.Hora
            );

        // ============================
        // 🔴 SLOT JÁ EXISTE
        // ============================
        if (existente != null)
        {
            // 🚫 NÃO pode reativar se existir agendamento pendente ou confirmado
            var possuiAgendamentoAtivo = await _repositorio.Agendamentos
                .AnyAsync(a =>
                    a.DisponibilidadeId == existente.Id &&
                    (a.Status == StatusAgendamento.Pendente ||
                     a.Status == StatusAgendamento.Confirmado)
                );

            if (possuiAgendamentoAtivo)
                throw new AppException(
                    "Horário possui agendamento ativo e não pode ser reativado.",
                    409
                );

            existente.Ativo = true;
            await _repositorio.SaveChangesAsync();
            return;
        }

        // ============================
        // 🟢 SLOT NOVO
        // ============================
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

    public async Task AtualizarStatusAsync(int disponibilidadeId, bool ativo, int barbeiroId)
    {
        var disponibilidade = await _repositorio.Disponibilidades
            .FirstOrDefaultAsync(x =>
                x.Id == disponibilidadeId &&
                x.BarbeiroId == barbeiroId
            );

        if (disponibilidade == null)
            throw new AppException("Disponibilidade não encontrada", 404);

        // 🚫 Bloqueia reativação manual se houver agendamento ativo
        if (ativo)
        {
            var possuiAgendamentoAtivo = await _repositorio.Agendamentos
                .AnyAsync(a =>
                    a.DisponibilidadeId == disponibilidade.Id &&
                    (a.Status == StatusAgendamento.Pendente ||
                     a.Status == StatusAgendamento.Confirmado)
                );

            if (possuiAgendamentoAtivo)
                throw new AppException(
                    "Horário possui agendamento ativo e não pode ser reativado.",
                    409
                );
        }

        disponibilidade.Ativo = ativo;
        await _repositorio.SaveChangesAsync();
    }

    public async Task<bool> ReservarSlotAsync(int barbeiroId, DateOnly data, string hora)
    {
        var slot = await _repositorio.Disponibilidades.FirstOrDefaultAsync(x =>
            x.BarbeiroId == barbeiroId &&
            x.Data == data &&
            x.Hora == hora &&
            x.Ativo
        );

        if (slot == null)
            return false;

        slot.Ativo = false;
        await _repositorio.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LiberarSlotAsync(int barbeiroId, DateOnly data, string hora)
    {
        var slot = await _repositorio.Disponibilidades.FirstOrDefaultAsync(x =>
            x.BarbeiroId == barbeiroId &&
            x.Data == data &&
            x.Hora == hora &&
            !x.Ativo
        );

        if (slot == null)
            return false;

        slot.Ativo = true;
        await _repositorio.SaveChangesAsync();
        return true;
    }
}
