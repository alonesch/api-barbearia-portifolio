using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.DTOs;
using System.Collections;
using System.Threading.Tasks;



namespace BarbeariaPortifolio.API.Servicos.Interfaces
{
    public interface IUsuarioServico
    {
        Task<IEnumerable<UsuarioDTO>> ListarTodos();
        Task<Usuario?> BuscarPorId(int id);
        Task<Usuario> Cadastrar(Usuario usuario);
        Task<bool> Atualizar(int id, Usuario usuario);
        Task<bool> Excluir(int id);
    }
}