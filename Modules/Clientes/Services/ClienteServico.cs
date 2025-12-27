using BarbeariaPortifolio.API.Modules.Clientes.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Clientes.Models;
using BarbeariaPortifolio.API.Modules.Clientes.Repositories.Interfaces;
using BarbeariaPortifolio.API.Modules.Clientes.Services.Interfaces;

namespace BarbeariaPortifolio.API.Modules.Clientes.Services;

public class ClienteServico : IClienteServico
{
    private readonly IClienteRepositorio _repositorio;

    public ClienteServico(IClienteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ClienteDTO> BuscarPorUsuario(int usuarioId)
    {
        var cliente = await _repositorio.BuscarPorUsuario(usuarioId);

        if (cliente == null)
            throw new AppException("Perfil de cliente não encontrado", 404);

        return new ClienteDTO
        {
            UsuarioId = cliente.UsuarioId,
            NomeCompleto = cliente.Usuario?.NomeCompleto ?? "",
            Email = cliente.Usuario?.Email ?? "",
            Cpf = cliente.Cpf,
            Telefone = cliente.Usuario?.Telefone ?? "",
            DataCadastro = cliente.DataCadastro,
            FotoPerfilUrl = cliente.Usuario?.FotoPerfilUrl
           
        };
    }

    public async Task<ClienteDTO> CriarPerfil(int usuarioId, ClienteDTO dto)
    {

        var existente = await _repositorio.BuscarPorUsuario(usuarioId);
        if (existente != null)
            throw new AppException("Perfil de cliente já existe", 409);

        var novo = new Cliente
        {
            UsuarioId = usuarioId,
            Cpf = dto.Cpf,
            DataCadastro = DateTime.UtcNow
        };

        var criado = await _repositorio.Criar(novo);

        return new ClienteDTO
        {
            UsuarioId = criado.UsuarioId,
            NomeCompleto = criado.Usuario?.NomeCompleto ?? "",
            Email = criado.Usuario?.Email ?? "",
            Cpf = criado.Cpf,
            Telefone = criado.Usuario?.Telefone ?? "",
            DataCadastro = criado.DataCadastro
        };
    }

    public async Task<bool> AtualizarPerfil(int usuarioId, ClienteDTO dto)
    {
        var cliente = await _repositorio.BuscarPorUsuario(usuarioId);

        if (cliente == null)
            throw new AppException("Perfil de cliente não encontrado", 404);

        cliente.Usuario.Telefone = dto.Telefone;
        cliente.Cpf = dto.Cpf;

        return await _repositorio.Atualizar(cliente);
    }
}
