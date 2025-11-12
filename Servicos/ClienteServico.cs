using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos;

public class ClienteServico : IClienteServico
{
    private readonly IClienteRepositorio _repositorio;

    public ClienteServico(IClienteRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<ClienteDTO>> ListarTodos()
    {
        var clientes = await _repositorio.ListarTodos();
        return clientes.Select(c => new ClienteDTO
        {
            Id = c.Id,
            Nome = c.Nome,
            Cpf = c.Cpf,
            Telefone = c.Telefone,
            DataCadastro = c.DataCadastro
        });
    }

    public async Task<ClienteDTO?> BuscarPorId(int id)
    {
        var cliente = await _repositorio.BuscarPorId(id);
        if (cliente == null) return null;

        return new ClienteDTO
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Telefone = cliente.Telefone,
            DataCadastro = cliente.DataCadastro
        };
    }

    public async Task<ClienteDTO> Cadastrar(ClienteDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new Exception("O nome do cliente é obrigatório");

        if (string.IsNullOrWhiteSpace(dto.Telefone))
            throw new Exception("O telefone do cliente é obrigatório");

        var novo = new Cliente
        {
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            Telefone = dto.Telefone,
            DataCadastro = DateTime.Now
        };

        var criado = await _repositorio.Cadastrar(novo);

        return new ClienteDTO
        {
            Id = criado.Id,
            Nome = criado.Nome,
            Cpf = criado.Cpf,
            Telefone = criado.Telefone,
            DataCadastro = criado.DataCadastro
        };
    }

    public async Task<bool> Atualizar(int id, ClienteDTO dto)
    {
        var clienteExistente = await _repositorio.BuscarPorId(id);
        if (clienteExistente == null) return false;

        clienteExistente.Nome = dto.Nome;
        clienteExistente.Cpf = dto.Cpf;
        clienteExistente.Telefone = dto.Telefone;

        return await _repositorio.Atualizar(id, clienteExistente);
    }

    public async Task<bool> Excluir(int id)
        => await _repositorio.Excluir(id);
}
