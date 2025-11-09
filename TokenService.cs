using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BarbeariaPortifolio.API.Auth
{
    public class TokenService
    {
        private readonly JwtOptions _jwt;

        public TokenService(IOptions<JwtOptions> jwt)
        {
            _jwt = jwt.Value;
        }

        // Gera o Access Token a partir de uma lista de claims
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(_jwt.ExpiresInHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Gera a string crua do refresh token (para retornar ao cliente UMA vez)
        public string GenerateRefreshTokenRaw(int bytesLength = 64)
        {
            var bytes = RandomNumberGenerator.GetBytes(bytesLength);
            // base64url para evitar problemas com transporte
            return Base64UrlEncoder.Encode(bytes);
        }

        // Hash do refresh token para salvar no banco
        public string HashRefreshToken(string refreshTokenRaw)
        {
            // SHA256 -> string hex/base64 (pode trocar por BCrypt se preferir)
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRaw));
            return Convert.ToHexString(hash); // HEX maiúsculo
        }
    }
}
