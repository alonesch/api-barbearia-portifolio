namespace BarbeariaPortifolio.API.Auth
{
    public class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;

        // usado pelo access token (em horas)
        public int ExpiresInHours { get; set; } = 2;
        public int RefreshTokenDays { get; set; } = 7;
    }
}
