using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Exceptions;

namespace BarbeariaPortifolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicoController : ControllerBase
    {
        private readonly IServicoServico _servicoServico;

        public ServicoController(IServicoServico servicoServico)
        {
            _servicoServico = servicoServico;
        }


        [Authorize]
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
                throw new AppException("Serviço não encontrado.", 404);

            return Ok(servico);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] ServicoDTO dto)
        {
            var novo = await _servicoServico.Cadastrar(dto);

            return CreatedAtAction(nameof(Buscar), new { id = novo.Id }, new
            {
                mensagem = "Serviço cadastrado com sucesso.",
                dados = novo
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ServicoDTO dto)
        {
            var atualizado = await _servicoServico.Atualizar(id, dto);
            if (!atualizado)
                throw new AppException("Serviço não encontrado.", 404);

            return Ok(new { mensagem = "Serviço atualizado com sucesso." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _servicoServico.Excluir(id);
            if (!excluido)
                throw new AppException("Serviço não encontrado.", 404);

            return Ok(new { mensagem = "Serviço excluído com sucesso." });
        }
    }
}
