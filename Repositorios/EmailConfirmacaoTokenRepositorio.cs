using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios
{
    public class EmailConfirmacaoTokenRepositorio : IEmailConfirmacaoTokenRepositorio
    {
        private readonly DataContext _repositorio;

        public EmailConfirmacaoTokenRepositorio(DataContext repositorio)
        {
            _repositorio = repositorio;
        }

        // ✅ Cria um novo token de confirmação
        public async Task CriarAsync(EmailConfirmacaoToken token)
        {
            _repositorio.EmailConfirmacaoTokens.Add(token);
            await _repositorio.SaveChangesAsync();
        }

        // ✅ Busca token pelo valor (usado na confirmação de e-mail)
        public async Task<EmailConfirmacaoToken?> BuscarPorTokenAsync(string token)
        {
            return await _repositorio.EmailConfirmacaoTokens
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        // ✅ Busca o último token gerado para um usuário (rate limit)
        public async Task<EmailConfirmacaoToken?> BuscarUltimoPorUsuarioAsync(int usuarioId)
        {
            return await _repositorio.EmailConfirmacaoTokens
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.CriadoEm)
                .FirstOrDefaultAsync();
        }

        // ✅ Invalida todos os tokens ativos (não usados) do usuário
        public async Task InvalidarTokensAtivosPorUsuarioAsync(int usuarioId)
        {
            var tokensAtivos = await _repositorio.EmailConfirmacaoTokens
                .Where(x => x.UsuarioId == usuarioId && !x.Usado)
                .ToListAsync();

            if (tokensAtivos.Count == 0)
                return;

            foreach (var token in tokensAtivos)
            {
                token.Usado = true;
            }

            await _repositorio.SaveChangesAsync();
        }

        // ✅ Persiste alterações no token (ex: marcar como usado)
        public async Task SalvarAsync(EmailConfirmacaoToken token)
        {
            _repositorio.EmailConfirmacaoTokens.Update(token);
            await _repositorio.SaveChangesAsync();
        }
    }
}
