using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios;


public class ClienteRepositorio : IClienteRepositorio
{
    private readonly DataContext _banco;


    public ClienteRepositorio(DataContext banco)
    {
        _banco = banco;
    }

    public async Task<IEnumerable<Cliente>> ListarTodos()
        => await _banco.Clientes.ToListAsync();

    public async Task<Cliente?> BuscarPorId(int id)
        => await _banco.Clientes.FindAsync(id);
    
    public async Task<Cliente?> BuscarPorNome(string nome)
    => await _banco.Clientes.FirstOrDefaultAsync(c => c.Nome == nome);


    public async Task<Cliente> Cadastrar(Cliente cliente)
    {
        _banco.Clientes.Add(cliente);
        await _banco.SaveChangesAsync();
        return cliente;
    }

    public async Task <bool> Atualizar(int id, Cliente cliente)
    {
        if (id != cliente.Id) return false;

        _banco.Entry(cliente).State = EntityState.Modified;
        await _banco.SaveChangesAsync();
        return true;  
    }

    public async Task <bool> Excluir(int id)
    {
        var cliente = await _banco.Clientes.FindAsync(id);
        if (cliente == null) return false;

        _banco.Clientes.Remove(cliente);
        await _banco.SaveChangesAsync();
        return true;
    }
}