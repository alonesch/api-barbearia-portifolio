using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class AgendamentoRepositorio : IAgendamentoRepositorio
    {
        private readonly DataContext _banco;

        public AgendamentoRepositorio(DataContext banco)
        {
            _banco = banco;
        }

        public async Task<IEnumerable<Agendamento>> ListarTodos()
        {
            return await _banco.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .ToListAsync();
        }

        public async Task<IEnumerable<Agendamento>> ListarPorBarbeiro(int barbeiroId)
        {
            return await _banco.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .Where(a => a.BarbeiroId == barbeiroId)
                .ToListAsync();
        }

        public async Task<Agendamento?> BuscarPorId(int id)
        {
            return await _banco.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Agendamento> Cadastrar(Agendamento agendamento)
        {
            _banco.Agendamentos.Add(agendamento);
            await _banco.SaveChangesAsync();
            return agendamento;
        }

        public async Task<bool> Atualizar(int id, Agendamento agendamento)
        {
            if (id != agendamento.Id)
                return false;

            _banco.Entry(agendamento).State = EntityState.Modified;
            await _banco.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var agendamento = await _banco.Agendamentos.FindAsync(id);
            if (agendamento == null) return false;

            _banco.Agendamentos.Remove(agendamento);
            await _banco.SaveChangesAsync();
            return true;
        }
    }
}
