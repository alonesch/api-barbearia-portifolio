using BarbeariaPortifolio.API.Models;


namespace BarbeariaPortifolio.API.Repositorios.Interfaces;

public interface IClienteRepositorio
{
    Task<IEnumerable<Cliente>> ListarTodos();
    Task<Cliente?> BuscarPorId(int id);
    Task<Cliente?> BuscarPorNome(string nome);
    Task<Cliente> Cadastrar(Cliente cliente);
    Task<bool> Atualizar(int id, Cliente cliente);
    Task<bool> Excluir(int id);

}