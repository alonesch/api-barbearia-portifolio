using BarbeariaPortifolio.API.Modules.Barbeiros.Repositories.Interfaces;
using BarbeariaPortifolio.API.Modules.Barbeiros.Services.Interfaces;
using BarbeariaPortifolio.API.Modules.Barbeiros.Models;
using BarbeariaPortifolio.API.Modules.Barbeiros.DTOs;
using BarbeariaPortifolio.API.Modules.Agendamentos.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;


namespace BarbeariaPortifolio.API.Modules.Barbeiros.Services;

public class BarbeiroServico : IBarbeiroServico
{
    private readonly IBarbeiroRepositorio _repositorio;

    public BarbeiroServico(IBarbeiroRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<BarbeiroDTO>> ListarTodos()
    {
        var lista = await _repositorio.ListarTodos();

        return lista.Select(b => new BarbeiroDTO
        {
            Id = b.Id,
            Nome = b.Nome,
            Telefone = b.Usuario?.Telefone ?? string.Empty,
            Usuario = b.Usuario?.NomeUsuario,
            Agendamentos = b.Agendamentos?.Select(a => new AgendamentoDTO
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                Nome = a.Usuario.NomeCompleto,
                Email = a.Usuario.Email,
                BarbeiroId = a.BarbeiroId,
                DataHora = a.DataHora,
                Status = (int)a.Status,
                Observacao = a.Observacao,
                AgendamentoServicos = a.AgendamentoServicos.Select(s => new AgendamentoServicoDTO
                {
                    ServicoId = s.ServicoId,
                    Observacao = s.Observacao
                }).ToList()
            }).ToList()
        });
    }

    public async Task<BarbeiroDTO?> BuscarPorId(int id)
    {
        var b = await _repositorio.BuscarPorId(id);
        if (b == null) return null;

        return new BarbeiroDTO
        {
            Id = b.Id,
            Nome = b.Nome,
            Telefone = b.Usuario?.Telefone ?? string.Empty,
            Usuario = b.Usuario?.NomeUsuario,
            Agendamentos = b.Agendamentos?.Select(a => new AgendamentoDTO
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                Nome = a.Usuario.NomeCompleto,
                Email = a.Usuario.Email,
                BarbeiroId = a.BarbeiroId,
                DataHora = a.DataHora,
                Status = (int)a.Status,
                Observacao = a.Observacao,
                AgendamentoServicos = a.AgendamentoServicos.Select(s => new AgendamentoServicoDTO
                {
                    ServicoId = s.ServicoId,
                    Observacao = s.Observacao
                }).ToList()
            }).ToList()
        };
    }

    public async Task<BarbeiroDTO?> BuscarPorUsuarioId(int usuarioId)
    {
        var barbeiro = await _repositorio.BuscarPorUsuarioId(usuarioId);

        if (barbeiro == null)
            return null;

        return new BarbeiroDTO
        {
            Id = barbeiro.Id,
            Nome = barbeiro.Nome,
            Telefone = barbeiro.Usuario?.Telefone ?? string.Empty,
            Usuario = barbeiro.Usuario?.NomeUsuario
        };
    }


    public async Task<BarbeiroDTO> Cadastrar(CriarBarbeiroDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new AppException("O nome do barbeiro é obrigatório.", 400);

        var barbeiro = new Barbeiro
        {
            Nome = dto.Nome,
            UsuarioId = dto.UsuarioId
        };

        await _repositorio.Cadastrar(barbeiro);

        return new BarbeiroDTO
        {
            Id = barbeiro.Id,
            Nome = barbeiro.Nome,
            Telefone = barbeiro.Usuario?.Telefone ?? string.Empty,
            Usuario = barbeiro.Usuario?.NomeUsuario,
            Agendamentos = new List<AgendamentoDTO>()
        };
    }

    public async Task<bool> Atualizar(int id, CriarBarbeiroDTO dto)
    {
        var existente = await _repositorio.BuscarPorId(id);
        if (existente == null) return false;

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new AppException("O nome do barbeiro é obrigatório.", 400);


        existente.Nome = dto.Nome;
        existente.UsuarioId = dto.UsuarioId;

        return await _repositorio.Atualizar(id, existente);
    }

    public async Task<bool> Excluir(int id)
    {
        return await _repositorio.Excluir(id);
    }
}
