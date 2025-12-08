using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
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

        public async Task<IEnumerable<Barbeiro>> ListarTodos()
        {
            return await _banco.Barbeiros
                .Include(b => b.Usuario)
                .Include(b => b.Agendamentos!)
                    .ThenInclude(a => a.Usuario)                // ✅ em vez de Cliente
                .Include(b => b.Agendamentos!)
                    .ThenInclude(a => a.AgendamentoServicos!)
                        .ThenInclude(asv => asv.Servico)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Barbeiro?> BuscarPorId(int id)
        {
            return await _banco.Barbeiros
                .Include(b => b.Usuario)
                .Include(b => b.Agendamentos!)
                    .ThenInclude(a => a.Usuario)
                .Include(b => b.Agendamentos!)
                    .ThenInclude(a => a.AgendamentoServicos!)
                        .ThenInclude(asv => asv.Servico)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Barbeiro?> BuscarUsuarioId(int usuarioId)
        {
            return await _banco.Barbeiros
                .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
        }

        public async Task<Barbeiro?> BuscarPorNome(string nome)
        {
            return await _banco.Barbeiros
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Nome == nome);
        }

        public async Task<Barbeiro> Cadastrar(Barbeiro barbeiro)
        {
            _banco.Barbeiros.Add(barbeiro);
            await _banco.SaveChangesAsync();
            return barbeiro;
        }

        public async Task<bool> Atualizar(int id, Barbeiro barbeiro)
        {
            if (id != barbeiro.Id)
                return false;

            _banco.Entry(barbeiro).State = EntityState.Modified;
            await _banco.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var barbeiro = await _banco.Barbeiros.FindAsync(id);
            if (barbeiro == null)
                return false;

            _banco.Barbeiros.Remove(barbeiro);
            await _banco.SaveChangesAsync();
            return true;
        }
    }
}
