namespace BarbeariaPortifolio.API.Models
{
    public class EmailConfirmacaoToken
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string Token { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; }

        public DateTime ExpiraEm { get; set; }

        public bool Usado { get; set; }
    }
}
