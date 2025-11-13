namespace BarbeariaPortifolio.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? BarbeiroId { get; set; }
    }
}