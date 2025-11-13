using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class BarbeiroRepositorio : IBarbeiroRepositorio
    {
        private readonly DataContext _banco;

        public BarbeiroRepositorio(DataContext banco)
        {
            _banco = banco;
        }

        public async Task<IEnumerable<BarbeiroDTO>> ListarTodos()
        {
            return await _banco.Barbeiros
                .Include(b => b.Usuario)
                .Include(b => b.Agendamentos)
                .ThenInclude(a => a.Cliente)
                .Select(b => new BarbeiroDTO
                {
                    Id = b.Id,
                    Nome = b.Nome ?? string.Empty,
                    Telefone = b.Telefone,
                    Usuario = b.Usuario != null ? b.Usuario.NomeUsuario : null,
                    Agendamentos = b.Agendamentos != null
                        ? b.Agendamentos.Select(a => new AgendamentoDTO
                        {
                            Id = a.Id,
                            Cliente = a.Cliente.Nome,
                            Barbeiro = b.Nome ?? string.Empty,
                            DataHora = a.DataHora,
                            Status = a.Status,
                        }).ToList()
                        : new List<AgendamentoDTO>()
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Barbeiro?> BuscarPorId(int id)
            => await _banco.Barbeiros
                .Include(b => b.Usuario)
                .Include(b => b.Agendamentos)
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Barbeiro?> BuscarPorNome(string nome)
        {
            return await _banco.Barbeiros
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Nome != null && b.Nome.ToLower() == nome.ToLower());

        }

        public async Task<Barbeiro> Cadastrar(Barbeiro barbeiro)
        {
            _banco.Barbeiros.Add(barbeiro);
            await _banco.SaveChangesAsync();
            return barbeiro;
        }

        public async Task<bool> Atualizar(int id, Barbeiro barbeiro)
        {
            if (id != barbeiro.Id) return false;

            _banco.Entry(barbeiro).State = EntityState.Modified;
            await _banco.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var barbeiro = await _banco.Barbeiros.FindAsync(id);
            if (barbeiro == null) return false;

            _banco.Barbeiros.Remove(barbeiro);
            await _banco.SaveChangesAsync();
            return true;
        }
    }
}
