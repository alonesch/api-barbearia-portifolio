using BarbeariaPortifolio.API.Shared.Data;
using BarbeariaPortifolio.API.Modules.Auth.Models;
using BarbeariaPortifolio.API.Modules.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Modules.Auth.Repositories;

public class EmailConfirmacaoTokenRepositorio : IEmailConfirmacaoTokenRepositorio
{
    private readonly DataContext _context;

    public EmailConfirmacaoTokenRepositorio(DataContext context)
    {
        _context = context;
    }

    public async Task CriarAsync(EmailConfirmacaoToken token)
    {
        _context.EmailConfirmacaoTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<EmailConfirmacaoToken?> BuscarPorTokenAsync(string token)
    {
        return await _context.EmailConfirmacaoTokens
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<EmailConfirmacaoToken?> BuscarUltimoPorUsuarioAsync(int usuarioId)
    {
        return await _context.EmailConfirmacaoTokens
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.CriadoEm)
            .FirstOrDefaultAsync();
    }

    public async Task InvalidarTokensAtivosPorUsuarioAsync(int usuarioId)
    {
        var tokens = await _context.EmailConfirmacaoTokens
            .Where(t => t.UsuarioId == usuarioId && !t.Usado)
            .ToListAsync();

        foreach (var token in tokens)
            token.Usado = true;

        await _context.SaveChangesAsync();
    }

    public async Task SalvarAsync(EmailConfirmacaoToken token)
    {
        await _context.SaveChangesAsync();
    }

    public async Task RemoverTokensExpiradosOuUsadosAsync()
    {
        var agora = DateTime.UtcNow;

        var tokens = _context.EmailConfirmacaoTokens
            .Where(t => t.Usado || t.ExpiraEm < agora);

        _context.EmailConfirmacaoTokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }
}