using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly DataContext _banco;

        public UsuarioRepositorio(DataContext banco)
        {
            _banco = banco;
        }

        
        public async Task<IEnumerable<UsuarioDTO>> ListarTodos()
        {
            return await _banco.Usuarios
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    NomeUsuario = u.NomeUsuario,
                    Role = u.Role,
                    BarbeiroId = u.Barbeiro != null ? u.Barbeiro.Id : null
                })
                .AsNoTracking()
                .ToListAsync();
        }

        
        public async Task<Usuario?> BuscarPorId(int id)
        {
            return await _banco.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        
        public async Task<Usuario?> BuscarPorNome(string nomeUsuario)
        {
            return await _banco.Usuarios
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
