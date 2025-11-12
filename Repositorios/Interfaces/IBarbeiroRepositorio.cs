using BarbeariaPortifolio.API.Models;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IBarbeiroRepositorio
    {
        Task<Barbeiro?> BuscarPorNome(string nome);
    }
}
