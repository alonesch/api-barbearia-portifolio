using BarbeariaPortifolio.API.DTOs;
using BarbeariaPortifolio.API.Exceptions;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Models.Enums;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Data;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Servicos
{
    public class AgendamentoService : IAgendamentoServico
    {
        private readonly IAgendamentoRepositorio _repositorio;
        private readonly DataContext _context;

        public AgendamentoService(IAgendamentoRepositorio repositorio, DataContext context)
        {
            _repositorio = repositorio;
            _context = context;
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarTodos()
        {
            var agendamentos = await _repositorio.ListarTodos();
            return agendamentos.Select(MapToDto);
        }

        public async Task<AgendamentoDTO> BuscarPorId(int id)
        {
            var agendamento = await _repositorio.BuscarPorId(id);
            if (agendamento == null)
                throw new AppException("Agendamento não encontrado.", 404);

            return MapToDto(agendamento);
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiro(int barbeiroId)
        {
            var agendamentos = await _repositorio.ListarPorBarbeiro(barbeiroId);
            return agendamentos.Select(MapToDto);
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarPorUsuario(int usuarioId)
        {
            var agendamentos = await _repositorio.ListarPorUsuario(usuarioId);
            return agendamentos.Select(MapToDto);
        }

        public async Task<AgendamentoDTO> Cadastrar(int usuarioId, CriarAgendamentoDTO dto)
        {
            if (dto.DisponibilidadeId <= 0)
                throw new AppException("Disponibilidade inválida.", 400);

            if (dto.AgendamentoServicos == null || dto.AgendamentoServicos.Count == 0)
                throw new AppException("Selecione pelo menos um serviço.", 400);

            var slot = await _context.Disponibilidades
                .FirstOrDefaultAsync(x => x.Id == dto.DisponibilidadeId);

            if (slot == null)
                throw new AppException("Horário não encontrado.", 404);

            if (!slot.Ativo)
                throw new AppException("Este horário já está reservado.", 409);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                slot.Ativo = false;

                var dataHoraUtc = DateTime.SpecifyKind(
                    slot.Data.ToDateTime(TimeOnly.Parse(slot.Hora)),
                    DateTimeKind.Utc
                );

                var conflito = await _repositorio.ChecarHorarios(slot.BarbeiroId, dataHoraUtc);
                if (conflito)
                    throw new AppException("Horário já reservado!", 409);

                var agendamento = new Agendamento
                {
                    UsuarioId = usuarioId,
                    BarbeiroId = slot.BarbeiroId,
                    DisponibilidadeId = slot.Id,
                    DataHora = dataHoraUtc,
                    DataRegistro = DateTime.UtcNow,
                    Status = StatusAgendamento.Pendente,
                    Observacao = dto.Observacao
                };

                _context.Agendamentos.Add(agendamento);
                await _context.SaveChangesAsync();

                foreach (var s in dto.AgendamentoServicos)
                {
                    var servicoExiste = await _context.Servicos
                        .AnyAsync(x => x.Id == s.ServicoId);

                    if (!servicoExiste)
                        throw new AppException("Serviço inválido.", 400);

                    _context.AgendamentoServicos.Add(new AgendamentoServico
                    {
                        AgendamentoId = agendamento.Id,
                        ServicoId = s.ServicoId,
                        Observacao = s.Observacao
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var usuario = await _context.Usuarios
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == usuarioId);

                return new AgendamentoDTO
                {
                    Id = agendamento.Id,
                    UsuarioId = usuarioId,
                    Nome = usuario.NomeCompleto,
                    Email = usuario.Email,
                    BarbeiroId = agendamento.BarbeiroId,
                    DataHora = agendamento.DataHora,
                    Status = (int)agendamento.Status,
                    Observacao = agendamento.Observacao,
                    AgendamentoServicos = dto.AgendamentoServicos
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> Atualizar(int id, AgendamentoDTO dto)
        {
            var agendamento = await _repositorio.BuscarPorId(id);
            if (agendamento == null)
                throw new AppException("Agendamento não encontrado.", 404);

            if (!Enum.IsDefined(typeof(StatusAgendamento), dto.Status))
                throw new AppException("Status inválido.", 400);

            agendamento.DataHora = dto.DataHora.ToUniversalTime();
            agendamento.Status = (StatusAgendamento)dto.Status;
            agendamento.Observacao = dto.Observacao;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AlterarStatus(int id, int novoStatus)
        {
            var agendamento = await _repositorio.BuscarStatusId(id);
            if (agendamento == null)
                throw new AppException("Agendamento não encontrado.", 404);

            if (!Enum.IsDefined(typeof(StatusAgendamento), novoStatus))
                throw new AppException("Status inválido.", 400);

            agendamento.Status = (StatusAgendamento)novoStatus;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task CancelarAgendamento(int id, int usuarioId)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Disponibilidade)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
                throw new AppException("Agendamento não encontrado.", 404);

            if (agendamento.UsuarioId != usuarioId)
                throw new AppException("Você não pode cancelar um agendamento de outro cliente.", 403);

            if (agendamento.DataHora <= DateTime.UtcNow)
                throw new AppException("Não é possível cancelar após o horário marcado.", 400);

            if (agendamento.Status != StatusAgendamento.Pendente &&
                agendamento.Status != StatusAgendamento.Confirmado)
                throw new AppException("Este agendamento não pode mais ser cancelado.", 400);

            agendamento.Status = StatusAgendamento.Cancelado;

            if (agendamento.Disponibilidade != null)
                agendamento.Disponibilidade.Ativo = true;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> Excluir(int id)
        {
            var agendamento = await _repositorio.BuscarPorId(id);
            if (agendamento == null)
                throw new AppException("Agendamento não encontrado.", 404);

            if (agendamento.DisponibilidadeId > 0)
            {
                var slot = await _context.Disponibilidades
                    .FirstOrDefaultAsync(x => x.Id == agendamento.DisponibilidadeId);

                if (slot != null)
                    slot.Ativo = true;
            }

            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();
            return true;
        }

        private static AgendamentoDTO MapToDto(Agendamento a)
        {
            return new AgendamentoDTO
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                Nome = a.Usuario.NomeCompleto,
                Email = a.Usuario.Email,
                BarbeiroId = a.BarbeiroId,
                DataHora = a.DataHora,
                Status = (int)a.Status,
                Observacao = a.Observacao,
                AgendamentoServicos = a.AgendamentoServicos.Select(s => new AgendamentoServicoDTO
                {
                    ServicoId = s.ServicoId,
                    Observacao = s.Observacao
                }).ToList()
            };
        }
        public async Task<PagedResultDTO<AgendamentoDTO>> ListarPorUsuarioPaginado(int usuarioId, int page, int pageSize)
        {
            if (page < 1)
                throw new AppException("Página inválida.", 400);

            if (pageSize < 1)
                pageSize = 10;

            if (pageSize > 30)
                pageSize = 30;

            var query = _repositorio.QueryPorUsuario(usuarioId)
                .OrderByDescending(a => a.DataHora);

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDTO<AgendamentoDTO>
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Items = items.Select(MapToDto)
            };
        }

    }
}
