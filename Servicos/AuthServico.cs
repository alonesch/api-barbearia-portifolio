using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Servicos
{
    public class AuthServico : IAuthServico
    {
        private readonly IUsuarioRepositorio _usuarios;
        private readonly IRefreshTokenRepositorio _refreshTokens;
        private readonly TokenService _tokenService;
        private readonly JwtOptions _jwt;
        private readonly IBarbeiroRepositorio _barbeiros;

        public AuthServico(
            IUsuarioRepositorio usuarios,
            IBarbeiroRepositorio barbeiros,
            IRefreshTokenRepositorio refreshTokens,
            TokenService tokenService,
            IOptions<JwtOptions> jwt)
        {
            _usuarios = usuarios;
            _refreshTokens = refreshTokens;
            _tokenService = tokenService;
            _jwt = jwt.Value;
            _barbeiros = barbeiros;
        }

        public async Task<(bool sucesso, string mensagem, Usuario? usuario)> ValidarLogin(string usuario, string senha)
        {
            var user = await _usuarios.BuscarPorNome(usuario);

            if (user == null || !user.Ativo)
                return (false, "Usuário ou senha incorretos.", null);

            if (!BCrypt.Net.BCrypt.Verify(senha, user.SenhaHash))
                return (false, "Usuário ou senha incorretos.", null);

            return (true, "Login realizado com sucesso.", user);
        }

        public async Task<string> GerarAccessToken(Usuario usuario)
        {
            var barbeiroId = await BuscarBarbeiroId(usuario.Id);
            var claims =  usuario.ToClaims(barbeiroId);
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

        public async Task<int?> BuscarBarbeiroId (int usuarioId)
        {
            var barbeiro = await _barbeiros.BuscarUsuarioId(usuarioId);

            return barbeiro?.Id;
        }
    }
}
