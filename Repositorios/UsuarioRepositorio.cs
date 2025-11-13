using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly DataContext _banco;

        public UsuarioRepositorio(DataContext banco)
        {
            _banco = banco;
        }

        // RETORNA MODEL, NÃO DTO
        public async Task<IEnumerable<Usuario>> ListarTodos()
        {
            return await _banco.Usuarios
                .Include(u => u.Barbeiro)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> BuscarPorId(int id)
        {
            return await _banco.Usuarios
                .Include(u => u.Barbeiro)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> BuscarPorNome(string nomeUsuario)
        {
            return await _banco.Usuarios
                .Include(u => u.Barbeiro)
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.NomeUsuario.ToLower() == nomeUsuario.ToLower()
                );
        }

        public async Task<Usuario> Cadastrar(Usuario usuario)
        {
            _banco.Usuarios.Add(usuario);
            await _banco.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> Atualizar(int id, Usuario usuario)
        {
            if (id != usuario.Id)
                return false;

            _banco.Entry(usuario).State = EntityState.Modified;
            await _banco.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var usuario = await _banco.Usuarios.FindAsync(id);
            if (usuario == null)
                return false;

            _banco.Usuarios.Remove(usuario);
            await _banco.SaveChangesAsync();
            return true;
        }
    }
}
