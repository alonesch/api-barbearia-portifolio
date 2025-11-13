using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        return cliente == null ? NotFound("Cliente não encontrado.") : Ok(cliente);
    }

    
    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] ClienteDTO dto)
    {
        var novo = await _clienteServico.Cadastrar(dto);
        return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, novo);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ClienteDTO dto)
    {
        var ok = await _clienteServico.Atualizar(id, dto);
        return ok ? NoContent() : NotFound("Cliente não encontrado.");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var ok = await _clienteServico.Excluir(id);
        return ok ? Ok("Cliente removido com sucesso.") : NotFound("Cliente não encontrado.");
    }
}
