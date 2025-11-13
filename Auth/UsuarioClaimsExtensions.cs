using System.Security.Claims;
using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Auth
{
    public static class UsuarioClaimsExtensions
    {
        public static IEnumerable<Claim> ToClaims(this Usuario user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.NomeUsuario),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("cargo", user.Cargo ?? ""),
                new Claim("nomeCompleto", user.NomeCompleto ?? "")
            };
        }
    }
}
