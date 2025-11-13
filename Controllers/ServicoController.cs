using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BarbeariaPortifolio.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicoController : ControllerBase
    {
        private readonly IServicoServico _servicoServico;

        public ServicoController(IServicoServico servicoServico)
        {
            _servicoServico = servicoServico;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var servicos = await _servicoServico.ListarTodos();
            return Ok(servicos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var servico = await _servicoServico.BuscarPorId(id);
            if (servico == null)
               return NotFound("Serviço não encontrado.");
            return Ok(servico);
        }

        
        [HttpPost]
        public async Task<IActionResult> Cadastrar (Servico servico)
        {
            var novo = await _servicoServico.Cadastrar(servico);
            return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, novo);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar (int id, Servico servico)
        {
            var atualizado = await _servicoServico.Atualizar(id, servico);
            if (!atualizado)
                return BadRequest("Erro ao atualizar o serviço.");
            return Ok("Servico atualizado com sucesso.");
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir (int id)
        {
            var excluido = await _servicoServico.Excluir(id);
            if (!excluido)
                return NotFound("Serviço não encontrado");
            return Ok("Serviço deletado com sucesso.");
        }

    }
}