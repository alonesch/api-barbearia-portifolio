namespace BarbeariaPortifolio.API.DTOs
{
    public class CriarBarbeiroDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public int? UsuarioId { get; set; }
    }
}
