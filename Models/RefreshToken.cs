using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarbeariaPortifolio.API.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(300)]
        public string TokenHash { get; set; } = null!; 

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool Revoked { get; set; } = false;

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;
    }
}
