using BarbeariaPortifolio.API.Modules.Usuarios.Repositories.Interfaces;
using BarbeariaPortifolio.API.Modules.Usuarios.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Usuarios.DTOs;

namespace BarbeariaPortifolio.API.Modules.Usuarios.Services;

public class UsuarioServico : IUsuarioServico
{
    private readonly IUsuarioRepositorio _repositorio;

    public UsuarioServico(IUsuarioRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    // Futuro:
    // public async Task<IEnumerable<Usuario>> ListarTodos()
    //     => await _repositorio.ListarTodos();



    public async Task<Usuario?> BuscarPorId(int id)
        => await _repositorio.BuscarPorId(id);

    public async Task<Usuario> Cadastrar(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
            throw new AppException("Nome do usuário é obrigatório.", 400);

        if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
            throw new AppException("Senha é obrigatória.", 400);



        return await _repositorio.Cadastrar(usuario);
    }

    public async Task<bool> Atualizar(int id, Usuario usuario)
    {
        if (id != usuario.Id)
            throw new AppException("O ID informado na rota não coincide com o ID do usuário.", 400);

        var existente = await _repositorio.BuscarPorId(id);
        if (existente == null)
            throw new AppException("Usuario não encontrado", 404);

        existente.NomeUsuario = usuario.NomeUsuario;
        existente.NomeCompleto = usuario.NomeCompleto;
        existente.Cargo = usuario.Cargo;
        existente.Ativo = usuario.Ativo;

        if (!string.IsNullOrWhiteSpace(usuario.SenhaHash))
            existente.SenhaHash = usuario.SenhaHash;

        return await _repositorio.Atualizar(id, existente);
    }


    //v2
    public async Task<UsuarioPerfilDTO> AtualizarMeAsync(
    int usuarioId,
    AtualizarUsuarioPerfilDTO dto
)
    {
        var usuario = await _repositorio.BuscarPorId(usuarioId)
            ?? throw new AppException("Usuário não encontrado.", 404);

        if (!string.IsNullOrWhiteSpace(dto.NomeCompleto))
            usuario.NomeCompleto = dto.NomeCompleto;

        if (!string.IsNullOrWhiteSpace(dto.Email))
            usuario.Email = dto.Email;

        if (!string.IsNullOrWhiteSpace(dto.Telefone))
            usuario.Telefone = dto.Telefone;

        await _repositorio.Atualizar(usuarioId, usuario);

        return new UsuarioPerfilDTO
        {
            Id = usuario.Id,
            NomeCompleto = usuario.NomeCompleto,
            NomeUsuario = usuario.NomeUsuario,
            Email = usuario.Email,
            Telefone = usuario.Telefone,
            Cargo = usuario.Cargo,
            FotoPerfilUrl = usuario.FotoPerfilUrl,
            BarbeiroId = usuario.Barbeiro?.Id
        };
    }


    public async Task AtualizarFotoPerfil(int usuarioId, string? fotoPerfilUrl)
    {
        var usuario = await _repositorio.BuscarPorId(usuarioId);
        if (usuario == null)
            throw new AppException("Usuário não encontrado.", 404);

        if (usuario.FotoPerfilUrl == fotoPerfilUrl)
            return;

        usuario.FotoPerfilUrl = fotoPerfilUrl;

        await _repositorio.Atualizar(usuario.Id, usuario);
    }


    public async Task<bool> Excluir(int id)
    {
        var existente = await _repositorio.BuscarPorId(id);
        if (existente == null)
            throw new AppException("Usuario não encontrado", 404);

        return await _repositorio.Excluir(id);

    }
}
