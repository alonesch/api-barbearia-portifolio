using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class BarbeiroRepositorio : IBarbeiroRepositorio
    {
        private readonly DataContext _banco;
        public BarbeiroRepositorio(DataContext banco) => _banco = banco;

        public async Task<Barbeiro?> BuscarPorNome(string nome)
            => await _banco.Barbeiros.FirstOrDefaultAsync(b => b.Nome == nome);
    }
}
