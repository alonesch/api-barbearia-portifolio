using Microsoft.EntityFrameworkCore;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;


namespace BarbeariaPortifolio.API.Modules.Auth.Models;

[Index(nameof(Token), IsUnique = true)]
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
