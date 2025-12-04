using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Exceptions;

namespace BarbeariaPortifolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteServico _clienteServico;

    public ClienteController(IClienteServico clienteServico)
    {
        _clienteServico = clienteServico;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await _clienteServico.ListarTodos());

    
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var cliente = await _clienteServico.BuscarPorId(id);
        if (cliente == null)
            throw new AppException("Cliente não encontrado.", 404);

        return Ok(cliente);
    }

    
    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] ClienteDTO dto)
    {
        var novo = await _clienteServico.Cadastrar(dto);
        return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, new
        {
            mensagem = "Cliente cadastrado com sucesso.",
            dados = novo
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ClienteDTO dto)
    {
        var ok = await _clienteServico.Atualizar(id, dto);
        if (!ok)
            throw new AppException("Cliente não encontrado.", 404);

        return Ok(new { mensagem = "Cliente atualizado com sucesso." });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var ok = await _clienteServico.Excluir(id);
        if (!ok)
            throw new AppException("Cliente não encontrado.", 404);

        return Ok(new { mensagem = "Cliente excluído com sucesso." });
    }
}
