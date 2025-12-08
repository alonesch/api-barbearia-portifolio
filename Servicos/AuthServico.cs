using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.DTOs;
using BarbeariaPortifolio.API.Exceptions;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.Extensions.Options;

namespace BarbeariaPortifolio.API.Servicos
{
    public class AuthServico : IAuthServico
    {
        private readonly IUsuarioRepositorio _usuarios;
        private readonly IRefreshTokenRepositorio _refreshTokens;
        private readonly TokenService _tokenService;
        private readonly JwtOptions _jwt;
        private readonly IBarbeiroRepositorio _barbeiros;
        private readonly IEmailConfirmacaoTokenRepositorio _emailTokens;
        private readonly IEmailServico _emailServico;

        public AuthServico(
            IUsuarioRepositorio usuarios,
            IBarbeiroRepositorio barbeiros,
            IRefreshTokenRepositorio refreshTokens,
            IEmailConfirmacaoTokenRepositorio emailTokens,
            IEmailServico emailServico,
            TokenService tokenService,
            IOptions<JwtOptions> jwt)
        {
            _usuarios = usuarios;
            _refreshTokens = refreshTokens;
            _tokenService = tokenService;
            _jwt = jwt.Value;
            _barbeiros = barbeiros;
            _emailTokens = emailTokens;
            _emailServico = emailServico;
        }

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

            var nomeExistente = await _usuarios.BuscarPorNome(dto.NomeUsuario);
            if (nomeExistente != null)
                throw new AppException("Nome de usuário já existe.", 409);

            var emailNormalizado = dto.Email.Trim().ToLowerInvariant();
            var emailExistente = await _usuarios.BuscarPorEmail(emailNormalizado);
            if (emailExistente != null)
                throw new AppException("Email já existe.", 409);

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

            var usuario = new Usuario
            {
                NomeCompleto = dto.NomeCompleto,
                NomeUsuario = dto.NomeUsuario,
                Email = emailNormalizado,
                SenhaHash = senhaHash,
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

            await _emailServico.EnviarEmailConfirmacaoAsync(usuario, token);

            return usuario;
        }

        public async Task<(string accessToken, string refreshToken, Usuario usuario)>
            LoginAsync(string login, string senha)
        {
            Usuario? user;

            if (login.Contains("@"))
                user = await _usuarios.BuscarPorEmail(login.Trim().ToLowerInvariant());
            else
                user = await _usuarios.BuscarPorNome(login);

            if (user == null || !user.Ativo)
                throw new AppException("Usuário ou senha inválidos.", 401);

            if (!BCrypt.Net.BCrypt.Verify(senha, user.SenhaHash))
                throw new AppException("Usuário ou senha inválidos.", 401);

            if (!user.EmailConfirmado)
                throw new AppException("Confirme seu email antes de logar", 403);

            var accessToken = await GerarAccessToken(user);
            var (refreshRaw, refreshHash) = await GerarRefreshToken();

            await SalvarRefreshToken(user, refreshHash, _jwt.RefreshTokenDays);

            return (accessToken, refreshRaw, user);
        }

        public async Task<string> GerarAccessToken(Usuario usuario)
        {
            var barbeiroId = await BuscarBarbeiroId(usuario.Id);
            var claims = usuario.ToClaims(barbeiroId);
            return _tokenService.GenerateAccessToken(claims);
        }

        public async Task<(string rawToken, string hashToken)> GerarRefreshToken()
        {
            var raw = _tokenService.GenerateRefreshTokenRaw();
            var hash = _tokenService.HashRefreshToken(raw);
            return (raw, hash);
        }

        public async Task SalvarRefreshToken(Usuario user, string hashToken, int diasExpiracao)
        {
            await _refreshTokens.RevogarTokensAtivos(user.Id);

            var rt = new RefreshToken
            {
                UsuarioId = user.Id,
                TokenHash = hashToken,
                ExpiraEm = DateTime.UtcNow.AddDays(diasExpiracao)
            };

            await _refreshTokens.Salvar(rt);
        }

        public async Task<int?> BuscarBarbeiroId(int usuarioId)
        {
            var barbeiro = await _barbeiros.BuscarUsuarioId(usuarioId);
            return barbeiro?.Id;
        }

        public async Task ReenviarConfirmacaoEmailAsync(ReenviarConfirmacaoEmailDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var usuario = await _usuarios.BuscarPorEmail(email);

            if (usuario == null || usuario.EmailConfirmado)
                return;

            var ultimoToken = await _emailTokens.BuscarUltimoPorUsuarioAsync(usuario.Id);

            if (ultimoToken != null &&
                !ultimoToken.Usado &&
                ultimoToken.CriadoEm > DateTime.UtcNow.AddMinutes(-2))
            {
                return;
            }

            await _emailTokens.InvalidarTokensAtivosPorUsuarioAsync(usuario.Id);

            var novoToken = new EmailConfirmacaoToken
            {
                UsuarioId = usuario.Id,
                Token = Guid.NewGuid().ToString("N"),
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.AddHours(1),
                Usado = false
            };

            await _emailTokens.CriarAsync(novoToken);

            await _emailServico.EnviarEmailConfirmacaoAsync(usuario, novoToken.Token);
        }

        public async Task ConfirmarEmailAsync(string token)
        {
            var confirmacao = await _emailTokens.BuscarPorTokenAsync(token);

            if (confirmacao == null ||
                confirmacao.Usado ||
                confirmacao.ExpiraEm < DateTime.UtcNow)
            {
                throw new AppException("Token inválido ou expirado.", 400);
            }

            confirmacao.Usado = true;
            confirmacao.Usuario.EmailConfirmado = true;

            await _emailTokens.SalvarAsync(confirmacao);
        }
    }
}
