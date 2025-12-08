using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
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
                .Include(a => a.Usuario) // ✅ em vez de Cliente
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(asg => asg.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Agendamento?> BuscarPorId(int id)
        {
            return await _repositorio.Agendamentos
                .Include(a => a.Usuario)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(asg => asg.Servico)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Agendamento>> ListarPorBarbeiro(int barbeiroId)
        {
            return await _repositorio.Agendamentos
                .Where(a => a.BarbeiroId == barbeiroId)
                .Include(a => a.Usuario)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(asg => asg.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Agendamento> Cadastrar(Agendamento agendamento)
        {
            _repositorio.Agendamentos.Add(agendamento);
            await _repositorio.SaveChangesAsync();
            return agendamento;
        }

        public async Task<bool> ChecarHorarios(int barbeiroId, DateTime dataHora)
        {
            return await _repositorio.Agendamentos.AnyAsync(
                a => a.BarbeiroId == barbeiroId &&
                     a.DataHora == dataHora.ToUniversalTime()
            );
        }

        public async Task<bool> Atualizar(Agendamento agendamento)
        {
            _repositorio.Agendamentos.Update(agendamento);
            await _repositorio.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var agendamento = await _repositorio.Agendamentos.FindAsync(id);
            if (agendamento == null) return false;

            _repositorio.Agendamentos.Remove(agendamento);
            await _repositorio.SaveChangesAsync();
            return true;
        }

        public async Task CadastrarAgendamentoServico(AgendamentoServico item)
        {
            _repositorio.AgendamentoServicos.Add(item);
            await _repositorio.SaveChangesAsync();
        }

        public async Task<Agendamento?> BuscarStatusId(int id)
        {
            return await _repositorio.Agendamentos
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AlterarStatus(Agendamento agendamento)
        {
            _repositorio.Agendamentos.Update(agendamento);
            await _repositorio.SaveChangesAsync();
        }
    }
}
