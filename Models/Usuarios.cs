using System.ComponentModel.DataAnnotations;

namespace BarbeariaPortifolio.API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        public string NomeCompleto { get; set; } = string.Empty;   
        public string Cargo { get; set; } = string.Empty;          

        public string Role { get; set; } = "cliente";              

        public bool Ativo { get; set; } = true;                    

        public int? BarbeiroId { get; set; }                       
        public Barbeiro? Barbeiro { get; set; }                    
    }
}
