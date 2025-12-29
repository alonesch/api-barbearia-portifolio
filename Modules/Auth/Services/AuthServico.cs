using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Auth.DTOs;
using BarbeariaPortifolio.API.Modules.Auth.Jwt;
using BarbeariaPortifolio.API.Modules.Auth.Models;
using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Modules.Auth.Repositories.Interfaces;
using BarbeariaPortifolio.API.Modules.Auth.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Barbeiros.Repositories.Interfaces;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using BarbeariaPortifolio.API.Modules.Usuarios.Repositories.Interfaces;
using Microsoft.Extensions.Options;


namespace BarbeariaPortifolio.API.Modules.Auth.Services;

public class AuthServico : IAuthServico
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IRefreshTokenRepositorio _refreshTokens;
    private readonly TokenService _tokenService;
    private readonly JwtOptions _jwt;
    private readonly IBarbeiroRepositorio _barbeiros;
    private readonly IEmailConfirmacaoTokenRepositorio _emailTokens;
    private readonly IEmailServico _emailServico;
    private readonly IConfiguration _config;

    public AuthServico(
        IUsuarioRepositorio usuarios,
        IBarbeiroRepositorio barbeiros,
        IRefreshTokenRepositorio refreshTokens,
        IEmailConfirmacaoTokenRepositorio emailTokens,
        IEmailServico emailServico,
        TokenService tokenService,
        IOptions<JwtOptions> jwt,
        IConfiguration config
    )
    {
        _usuarios = usuarios;
        _barbeiros = barbeiros;
        _refreshTokens = refreshTokens;
        _emailTokens = emailTokens;
        _emailServico = emailServico;
        _tokenService = tokenService;
        _jwt = jwt.Value;
        _config = config;
    }

    // ======================================================
    // REGISTRO
    // ======================================================
    public async Task<Usuario> RegistrarAsync(RegistrarNovoClienteDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NomeCompleto))
            throw new AppException("Nome completo é obrigatório.", 400);

        if (string.IsNullOrWhiteSpace(dto.NomeUsuario))
            throw new AppException("Nome de usuário é obrigatório.", 400);

        if (string.IsNullOrWhiteSpace(dto.Email))
            throw new AppException("Email é obrigatório.", 400);

        if (string.IsNullOrWhiteSpace(dto.Senha))
            throw new AppException("Senha é obrigatória.", 400);

        if (await _usuarios.BuscarPorNome(dto.NomeUsuario) != null)
            throw new AppException("Nome de usuário já existe.", 409);

        var email = dto.Email.Trim().ToLowerInvariant();
        if (await _usuarios.BuscarPorEmail(email) != null)
            throw new AppException("Email já existe.", 409);

        var usuario = new Usuario
        {
            NomeCompleto = dto.NomeCompleto,
            NomeUsuario = dto.NomeUsuario,
            Email = email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
            Cargo = "Cliente",
            EmailConfirmado = false,
            Ativo = true
        };

        await _usuarios.Cadastrar(usuario);

        var token = Guid.NewGuid().ToString("N");

        var confirmacao = new EmailConfirmacaoToken
        {
            UsuarioId = usuario.Id,
            Token = token,
            CriadoEm = DateTime.UtcNow,
            ExpiraEm = DateTime.UtcNow.AddHours(24),
            Usado = false
        };

        await _emailTokens.CriarAsync(confirmacao);


        var link = $"{_config["API_URL"]}/api/v2/auth/confirmar-email?token={token}";

        try
        {
            await _emailServico.EnviarConfirmacaoEmailAsync(usuario.Email, usuario.NomeCompleto, link);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EMAIL] Falha ao enviar confirmação: {ex.Message}");
        }

        return usuario;
    }

    //CHECK DE USERNAME E EMAIL
    public async Task<bool> UsernameDisponivel(string nomeUsuario)
    {
       var usuario = await _usuarios.BuscarPorNome(nomeUsuario);
            return usuario == null;
    }

    public async Task<bool> EmailDisponivel(string email)
    {
        var usuario = await _usuarios.BuscarPorEmail(email);
            return usuario == null;
    }

    // ======================================================
    // LOGIN
    // ======================================================
    public async Task<(string accessToken, string refreshToken, Usuario usuario)> LoginAsync(string login, string senha)
    {
        Usuario? user = login.Contains("@")
            ? await _usuarios.BuscarPorEmail(login.Trim().ToLowerInvariant())
            : await _usuarios.BuscarPorNome(login);

        if (user == null || !user.Ativo)
            throw new AppException("Usuário ou senha inválidos.", 401);

        if (!BCrypt.Net.BCrypt.Verify(senha, user.SenhaHash))
            throw new AppException("Usuário ou senha inválidos.", 401);

        if (!user.EmailConfirmado)
            throw new AppException("Confirme seu email antes de logar.", 403);

        var accessToken = await GerarAccessToken(user);
        var (raw, hash) = await GerarRefreshToken();

        await SalvarRefreshToken(user, hash, _jwt.RefreshTokenDays);

        return (accessToken, raw, user);
    }

    // ======================================================
    // TOKEN
    // ======================================================
    public async Task<string> GerarAccessToken(Usuario usuario)
    {
        var barbeiroId = await BuscarPorUsuarioId(usuario.Id);
        return _tokenService.GenerateAccessToken(usuario.ToClaims(barbeiroId));
    }

    public async Task<int?> BuscarPorUsuarioId(int usuarioId)
    {
        var barbeiro = await _barbeiros.BuscarPorUsuarioId(usuarioId);
        return barbeiro?.Id;
    }


    public async Task<(string rawToken, string hashToken)> GerarRefreshToken()
    {
        var raw = _tokenService.GenerateRefreshTokenRaw();
        return (raw, _tokenService.HashRefreshToken(raw));
    }

    public async Task SalvarRefreshToken(Usuario user, string hashToken, int diasExpiracao)
    {
        await _refreshTokens.RevogarTokensAtivos(user.Id);

        await _refreshTokens.Salvar(new RefreshToken
        {
            UsuarioId = user.Id,
            TokenHash = hashToken,
            ExpiraEm = DateTime.UtcNow.AddDays(diasExpiracao)
        });
    }



    // ======================================================
    // EMAIL - REENVIO
    // ======================================================
    public async Task ReenviarConfirmacaoEmailAsync(ReenviarConfirmacaoEmailDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarios.BuscarPorEmail(email);

        if (usuario == null || usuario.EmailConfirmado)
            return;

        var ultimo = await _emailTokens.BuscarUltimoPorUsuarioAsync(usuario.Id);

        if (ultimo != null && !ultimo.Usado &&
            ultimo.CriadoEm > DateTime.UtcNow.AddMinutes(-2))
            return;

        await _emailTokens.InvalidarTokensAtivosPorUsuarioAsync(usuario.Id);

        var token = new EmailConfirmacaoToken
        {
            UsuarioId = usuario.Id,
            Token = Guid.NewGuid().ToString("N"),
            CriadoEm = DateTime.UtcNow,
            ExpiraEm = DateTime.UtcNow.AddHours(24),
            Usado = false
        };

        await _emailTokens.CriarAsync(token);

        // ✅ URL CORRETA
        var link = $"{_config["API_URL"]}/api/v2/auth/confirmar-email?token={token.Token}";

        await _emailServico.EnviarConfirmacaoEmailAsync(usuario.Email, usuario.NomeCompleto, link);
    }

    // ======================================================
    // EMAIL - CONFIRMAR
    // ======================================================
    public async Task ConfirmarEmailAsync(string token)
    {
        var confirmacao = await _emailTokens.BuscarPorTokenAsync(token);

        if (confirmacao == null)
            throw new AppException("Token inválido.", 400);

        if (confirmacao.Usado)
            throw new AppException("Token já utilizado.", 409);

        if (confirmacao.ExpiraEm < DateTime.UtcNow)
            throw new AppException("Token expirado.", 410);

        if (confirmacao.Usuario.EmailConfirmado)
            throw new AppException("Email já confirmado.", 409);

        confirmacao.Usado = true;
        confirmacao.Usuario.EmailConfirmado = true;

        await _emailTokens.SalvarAsync(confirmacao);
    }
}
