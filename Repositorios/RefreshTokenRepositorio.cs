using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class RefreshTokenRepositorio : IRefreshTokenRepositorio
    {
        private readonly DataContext _banco;

        public RefreshTokenRepositorio(DataContext banco)
        {
            _banco = banco;
        }

        public async Task<RefreshToken?> BuscarPorHash(string hash)
        {
            return await _banco.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.TokenHash == hash);
        }

        public async Task<RefreshToken?> BuscarTokenValido(int usuarioId)
        {
            return await _banco.RefreshTokens
                .Where(r => r.UsuarioId == usuarioId && !r.Revogado && r.ExpiraEm > DateTime.UtcNow)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task Salvar(RefreshToken token)
        {
            _banco.RefreshTokens.Add(token);
            await _banco.SaveChangesAsync();
        }

        public async Task RevogarTokensAtivos(int usuarioId)
        {
            var tokens = _banco.RefreshTokens
                .Where(r => r.UsuarioId == usuarioId && !r.Revogado);

            await tokens.ExecuteUpdateAsync(t => t.SetProperty(x => x.Revogado, true));
        }
    }
}
