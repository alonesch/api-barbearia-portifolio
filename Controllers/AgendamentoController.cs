using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using BarbeariaPortifolio.API.DTOs;
using BarbeariaPortifolio.API.Exceptions;
using System.Security.Claims;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoServico _servico;

        public AgendamentoController(IAgendamentoServico servico)
        {
            _servico = servico;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var agendamentos = await _servico.ListarTodos();
            return Ok(agendamentos);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var agendamento = await _servico.BuscarPorId(id);
            return Ok(agendamento);
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("barbeiro/{id}")]
        public async Task<IActionResult> ListarPorBarbeiro(int id)
        {
            var agendamentos = await _servico.ListarPorBarbeiro(id);
            return Ok(agendamentos);
        }

        [Authorize(Policy = "Cliente")]
        [HttpGet("me")]
        public async Task<IActionResult> ListarMeus([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (page.HasValue)
            {
                var resultado = await _servico.ListarPorUsuarioPaginado(
                    usuarioId,
                    page.Value,
                    pageSize ?? 10
                );

                return Ok(resultado);
            }

            var agendamentos = await _servico.ListarPorUsuario(usuarioId);
            return Ok(agendamentos);
        }


        [Authorize(Policy = "Cliente")]
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] CriarAgendamentoDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var novo = await _servico.Cadastrar(usuarioId, dto);

            return CreatedAtAction(nameof(Buscar), new { Id = novo.Id }, new
            {
                mensagem = "Agendamento criado com sucesso!",
                dados = novo
            });
        }

        [Authorize(Policy = "Barbeiro")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AgendamentoDTO dto)
        {
            var atualizado = await _servico.Atualizar(id, dto);
            if (!atualizado)
                throw new AppException("Agendamento não encontrado.", 404);

            return Ok(new { mensagem = "Agendamento atualizado com sucesso." });
        }

        [Authorize]
        [HttpPatch("status/{id}")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] StatusDTO dto)
        {
            var alterado = await _servico.AlterarStatus(id, dto.Status);
            if (!alterado)
                throw new AppException("Agendamento não encontrado.", 404);

            return Ok(new { mensagem = "Status do agendamento atualizado com sucesso." });
        }

        [Authorize(Policy = "Cliente")]
        [HttpPatch("{id}/cancelar")]

        public async Task<IActionResult> Cancelar(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _servico.CancelarAgendamento(id, usuarioId);

            return Ok(new { mensagem = "Agendamento cancelado com sucesso." });
        }

        [Authorize(Policy = "AdminOuBarbeiro")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _servico.Excluir(id);
            if (!excluido)
                throw new AppException("Agendamento não encontrado.", 404);

            return Ok(new { mensagem = "Agendamento excluído com sucesso." });
        }
    }
}
