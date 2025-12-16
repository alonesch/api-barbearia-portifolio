using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Models.Enums;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class AgendamentoRepositorio : IAgendamentoRepositorio
    {
        private readonly DataContext _repositorio;

        public AgendamentoRepositorio(DataContext repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<Agendamento>> ListarTodos()
        {
            return await _repositorio.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.Barbeiro)
                .Include(a => a.Disponibilidade)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .ToListAsync();
        }

        public async Task<Agendamento?> BuscarPorId(int id)
        {
            return await _repositorio.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.Barbeiro)
                .Include(a => a.Disponibilidade)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Agendamento>> ListarHistoricoPorBarbeiro(int barbeiroId)
        {
            return await _repositorio.Agendamentos
                .Where(a =>
                    a.BarbeiroId == barbeiroId &&
                    a.Status == StatusAgendamento.Concluido
                )
                .Include(a => a.Usuario)
                .Include(a => a.Disponibilidade)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .ToListAsync();
        }


        public async Task<IEnumerable<Agendamento>> ListarPorUsuario(int usuarioId)
        {
            return await _repositorio.Agendamentos
                .Where(a => a.UsuarioId == usuarioId)
                .Include(a => a.Usuario)
                .Include(a => a.Barbeiro)
                .Include(a => a.Disponibilidade)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .ToListAsync();
        }

        public async Task<Agendamento> Cadastrar(Agendamento agendamento)
        {
            _repositorio.Agendamentos.Add(agendamento);
            return agendamento;
        }

        public async Task<bool> Atualizar(Agendamento agendamento)
        {
            _repositorio.Agendamentos.Update(agendamento);
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var agendamento = await _repositorio.Agendamentos.FindAsync(id);
            if (agendamento == null) return false;

            _repositorio.Agendamentos.Remove(agendamento);
            return true;
        }

        public async Task<bool> ChecarHorarios(int barbeiroId, DateTime dataHora, int disponibilidadeId)
        {
            return await _repositorio.Agendamentos
                .AnyAsync(a =>
                    a.BarbeiroId == barbeiroId &&
                    a.DataHora == dataHora &&
                    a.DisponibilidadeId != disponibilidadeId && // 🔑 CHAVE DA CORREÇÃO
                    (a.Status == StatusAgendamento.Pendente ||
                     a.Status == StatusAgendamento.Confirmado)
                );
        }


        public async Task CadastrarAgendamentoServico(AgendamentoServico item)
        {
            _repositorio.AgendamentoServicos.Add(item);
        }

        public async Task<Agendamento?> BuscarStatusId(int id)
        {
            return await _repositorio.Agendamentos
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AlterarStatus(Agendamento agendamento)
        {
            _repositorio.Agendamentos.Update(agendamento);
        }

        public IQueryable<Agendamento> QueryPorUsuario(int usuarioId)
        {
            return _repositorio.Agendamentos
                .Where(a => a.UsuarioId == usuarioId)
                .Include(a => a.Usuario)
                .Include(a => a.Barbeiro)
                .Include(a => a.Disponibilidade)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico);
        }
    }
}
