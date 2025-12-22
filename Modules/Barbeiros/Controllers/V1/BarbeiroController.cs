
using BarbeariaPortifolio.API.Modules.Barbeiros.DTOs;
using BarbeariaPortifolio.API.Shared.Exceptions;
using BarbeariaPortifolio.API.Modules.Barbeiros.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaPortifolio.API.Modules.Barbeiros.Controllers.V1;

[ApiController]
[Route("api/v1/barbeiros")]
public class BarbeiroController : ControllerBase
{
    private readonly IBarbeiroServico _servico;

    public BarbeiroController(IBarbeiroServico servico)
    {
        _servico = servico;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _servico.ListarTodos();
        return Ok(lista);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var item = await _servico.BuscarPorId(id);
        if (item == null)
            throw new AppException("Barbeiro não encontrado.", 404);

        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Cadastrar([FromBody] CriarBarbeiroDTO dto)
    {
        var novo = await _servico.Cadastrar(dto);

        return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, new
        {
            mensagem = "Barbeiro cadastrado com sucesso.",
            dado = novo
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarBarbeiroDTO dto)
    {
        var ok = await _servico.Atualizar(id, dto);

        if (!ok)
            throw new AppException("Barbeiro não encontrado.", 404);

        return Ok(new { mensagem = "Barbeiro atualizado com sucesso." });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var ok = await _servico.Excluir(id);

        if (!ok)
            throw new AppException("Barbeiro não encontrado.", 404);

        return Ok(new { mensagem = "Barbeiro excluído com sucesso." });
    }
}
