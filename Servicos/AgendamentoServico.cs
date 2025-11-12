using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Servicos;

public class AgendamentoServico : IAgendamentoServico
{
    private readonly IAgendamentoRepositorio _repositorio;
    private readonly IClienteRepositorio _clienteRepo;
    private readonly IBarbeiroRepositorio _barbeiroRepo;

    public AgendamentoServico(
        IAgendamentoRepositorio repositorio,
        IClienteRepositorio clienteRepo,
        IBarbeiroRepositorio barbeiroRepo)
    {
        _repositorio = repositorio;
        _clienteRepo = clienteRepo;
        _barbeiroRepo = barbeiroRepo;
    }

    public async Task<IEnumerable<AgendamentoDTO>> ListarTodos()
    {
        var agendamentos = await _repositorio.ListarTodos();

        return agendamentos.Select(a => new AgendamentoDTO
        {
            Id = a.Id,
            Cliente = a.Cliente.Nome,
            Barbeiro = a.Barbeiro.Nome,
            DataHora = a.DataHora,
            Status = a.Status,
            Observacao = a.Observacao,
            Servicos = a.AgendamentoServicos.Select(s => new ServicoDTO
            {
                NomeServico = s.Servico.NomeServico,
                Preco = s.Servico.Preco
            }).ToList()
        });
    }

    public async Task<AgendamentoDTO?> BuscarPorId(int id)
    {
        var a = await _repositorio.BuscarPorId(id);
        if (a == null) return null;

        return new AgendamentoDTO
        {
            Id = a.Id,
            Cliente = a.Cliente.Nome,
            Barbeiro = a.Barbeiro.Nome,
            DataHora = a.DataHora,
            Status = a.Status,
            Observacao = a.Observacao,
            Servicos = a.AgendamentoServicos.Select(s => new ServicoDTO
            {
                NomeServico = s.Servico.NomeServico,
                Preco = s.Servico.Preco
            }).ToList()
        };
    }

    public async Task<AgendamentoDTO> Cadastrar(AgendamentoDTO dto)
    {
        var cliente = await _clienteRepo.BuscarPorNome(dto.Cliente);

        var barbeiro = await _barbeiroRepo.BuscarPorNome(dto.Barbeiro);

        var agendamento = new Agendamento
        {
            ClienteId = cliente.Id,
            BarbeiroId = barbeiro.Id,
            DataHora = dto.DataHora,
            Status = dto.Status,
            Observacao = dto.Observacao,
            DataRegistro = DateTime.Now
        };

        var criado = await _repositorio.Cadastrar(agendamento);

        return await BuscarPorId(criado.Id) ?? throw new Exception("Erro ao criar agendamento");
    }

    public async Task<bool> Atualizar(int id, AgendamentoDTO dto)
    {
        var existente = await _repositorio.BuscarPorId(id);
        if (existente == null) return false;

        existente.Status = dto.Status;
        existente.Observacao = dto.Observacao;
        existente.DataHora = dto.DataHora;

        return await _repositorio.Atualizar(id, existente);
    }

    public async Task<bool> Excluir(int id)
        => await _repositorio.Excluir(id);
}
