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
                    .ThenInclude(asg => asg.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Agendamento?> BuscarPorId(int id)
        {
            return await _banco.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(asg => asg.Servico)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Agendamento>> ListarPorBarbeiro(int barbeiroId)
        {
            return await _banco.Agendamentos
                .Where(a => a.BarbeiroId == barbeiroId)
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(asg => asg.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Agendamento> Cadastrar(Agendamento agendamento)
        {
            _banco.Agendamentos.Add(agendamento);
            await _banco.SaveChangesAsync();
            return agendamento;
        }

        public async Task<bool> Atualizar(Agendamento agendamento)
        {
            _banco.Agendamentos.Update(agendamento);
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

        public async Task<Cliente> BuscarOuCriarCliente(string nome, string cpf, string telefone)
        {
            var cliente = await _banco.Clientes
                .FirstOrDefaultAsync(c => c.Telefone == telefone);

            if (cliente != null)
                return cliente;

            cliente = new Cliente
            {
                Nome = nome,
                Cpf = cpf,
                Telefone = telefone
            };

            _banco.Clientes.Add(cliente);
            await _banco.SaveChangesAsync();

            return cliente;
        }

        public async Task CadastrarAgendamentoServico(AgendamentoServico item)
        {
            _banco.AgendamentoServicos.Add(item);
            await _banco.SaveChangesAsync();
        }
    }
}
