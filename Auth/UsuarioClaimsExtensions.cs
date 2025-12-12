using BarbeariaPortifolio.API.Models;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Auth
{
    public static class UsuarioClaimsExtensions
    {
        public static IEnumerable<Claim> ToClaims(this Usuario user, int? barbeiroId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Cargo)
            };

            if (barbeiroId.HasValue)
                claims.Add(new Claim("barbeiroId", barbeiroId.Value.ToString()));

            return claims;
        }
    }
}
