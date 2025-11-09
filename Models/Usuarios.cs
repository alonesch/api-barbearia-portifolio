using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? NomeCompleto { get; set; }
        public string? Cargo { get; set; }
        public bool Ativo { get; set; } = true;
        public int? BarbeiroId { get; set; }
        public Barbeiro? Barbeiro { get; set; }

        // 🔹 Gera lista de Claims a partir do usuário logado
        public IEnumerable<Claim> ToClaims()
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Name, NomeUsuario),
                new Claim(ClaimTypes.Role, Cargo ?? "User")
            };
        }
    }
}
