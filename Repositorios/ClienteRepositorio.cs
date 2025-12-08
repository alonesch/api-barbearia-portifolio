using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios;

public class ClienteRepositorio : IClienteRepositorio
{
    private readonly DataContext _repositorio;

    public ClienteRepositorio(DataContext repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Cliente?> BuscarPorUsuario(int usuarioId)
    {
        return await _repositorio.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
    }

    public async Task<Cliente> Criar(Cliente cliente)
    {
        _repositorio.Clientes.Add(cliente);
        await _repositorio.SaveChangesAsync();
        return await _repositorio.Clientes
            .Include( c=> c.Usuario)
            .FirstAsync(c => c.Id == cliente.Id);
    }

    public async Task<bool> Atualizar(Cliente cliente)
    {
        _repositorio.Clientes.Update(cliente);
        await _repositorio.SaveChangesAsync();
        return true;
    }

    
    public async Task<Usuario> BuscarUsuarioPorId(int usuarioId)
    {
        return await _repositorio.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == usuarioId)
            ?? throw new Exception("Usuário não encontrado");
    }
}
