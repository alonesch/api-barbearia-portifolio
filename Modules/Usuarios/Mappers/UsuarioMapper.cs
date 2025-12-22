using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;

public static class UsuarioMapper
{
    public static UsuarioPerfilDTO ToPerfilDTO(Usuario usuario)
    {
        return new UsuarioPerfilDTO
        {
            Id = usuario.Id,
            NomeUsuario = usuario.NomeUsuario,
            NomeCompleto = usuario.NomeCompleto,
            Email = usuario.Email,
            Cargo = usuario.Cargo,
            FotoPerfilUrl = usuario.FotoPerfilUrl
        };
    }
}
