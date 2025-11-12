using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios;

public class AgendamentoRepositorio : IAgendamentoRepositorio
{
    private readonly DataContext _db;

    public AgendamentoRepositorio(DataContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Agendamento>> ListarTodos()
        => await _db.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Barbeiro)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(x => x.Servico)
            .ToListAsync();

    public async Task<Agendamento?> BuscarPorId(int id)
        => await _db.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Barbeiro)
            .Include(a => a.AgendamentoServicos)
                .ThenInclude(x => x.Servico)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Agendamento> Cadastrar(Agendamento agendamento)
    {
        _db.Agendamentos.Add(agendamento);
        await _db.SaveChangesAsync();
        return agendamento;
    }

    public async Task<bool> Atualizar(int id, Agendamento agendamento)
    {
        if (id != agendamento.Id) return false;

        _db.Entry(agendamento).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Excluir(int id)
    {
        var agendamento = await _db.Agendamentos.FindAsync(id);
        if (agendamento == null) return false;

        _db.Agendamentos.Remove(agendamento);
        await _db.SaveChangesAsync();
        return true;
    }
}
