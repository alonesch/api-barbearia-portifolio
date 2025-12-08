using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Repositorios.Interfaces
{
    public interface IEmailConfirmacaoTokenRepositorio
    {
        // ✅ Cria um novo token de confirmação de e-mail
        Task CriarAsync(EmailConfirmacaoToken token);

        // ✅ Busca um token específico pelo valor do token (usado na confirmação via link)
        Task<EmailConfirmacaoToken?> BuscarPorTokenAsync(string token);

        // ✅ Retorna o último token gerado para um usuário (usado no rate limit lógico)
        Task<EmailConfirmacaoToken?> BuscarUltimoPorUsuarioAsync(int usuarioId);

        // ✅ Invalida todos os tokens ativos (não usados) de um usuário
        Task InvalidarTokensAtivosPorUsuarioAsync(int usuarioId);

        // ✅ Persiste alterações (marcar como usado, expirar, etc.)
        Task SalvarAsync(EmailConfirmacaoToken token);
    }
}
