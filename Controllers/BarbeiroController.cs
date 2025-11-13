using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbeiroController : ControllerBase
    {
        private readonly IBarbeiroServico _Servico;
        public BarbeiroController(IBarbeiroServico Servico)
        {
            _Servico = Servico;
        }

        
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var barbeiros = await _Servico.ListarTodos();
            return Ok(barbeiros);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId (int id)
        {
            var barbeiro = await _Servico.BuscarPorId(id);
            if (barbeiro == null)
                return NotFound("Barbeiro não encontrado.");
            return Ok(barbeiro);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Cadastrar(Barbeiro barbeiro)
        {
            var novoBarbeiro = await _Servico.Cadastrar(barbeiro);
            return CreatedAtAction(nameof(BuscarPorId), new { id = novoBarbeiro.Id }, novoBarbeiro);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Barbeiro barbeiro)
        {
           var atualizado = await _Servico.Atualizar(id, barbeiro);
            if (!atualizado)
                return BadRequest("Não foi possível atualizar o barbeiro.");
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _Servico.Excluir(id);
            if (!excluido)
                return NotFound("Não foi possível excluir o barbeiro.");
            return NoContent();
        }
    }

}