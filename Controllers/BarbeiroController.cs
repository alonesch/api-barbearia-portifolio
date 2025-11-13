using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController : ControllerBase
    {
        private readonly IBarbeiroServico _servico;

        public BarbeiroController(IBarbeiroServico servico)
        {
            _servico = servico;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _servico.ListarTodos();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var item = await _servico.BuscarPorId(id);
            if (item == null) return NotFound("Barbeiro não encontrado.");
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] BarbeiroDTO dto)
        {
            var novo = await _servico.Cadastrar(dto);
            return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, novo);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] BarbeiroDTO dto)
        {
            var ok = await _servico.Atualizar(id, dto);
            return ok ? NoContent() : NotFound("Barbeiro não encontrado.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var ok = await _servico.Excluir(id);
            return ok ? Ok("Removido com sucesso.") : NotFound("Barbeiro não encontrado.");
        }
    }
}
