using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Exceptions;

namespace BarbeariaPortifolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServico _servico;

        public UsuarioController(IUsuarioServico servico)
        {
            _servico = servico;
        }

        // [Authorize]
        // [HttpGet]
        // public async Task<IActionResult> ListarTodos()
        //     => Ok(await _servico.ListarTodos());

        //[Authorize(Policy = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var usuario = await _servico.BuscarPorId(id);
            if (usuario == null)
                throw new AppException("Usuário não encontrado.", 404);

            return Ok(usuario);
        }

        //[Authorize(Policy = "AdminOuBarbeiro")]
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            var novo = await _servico.Cadastrar(usuario);

            return CreatedAtAction(nameof(BuscarPorId), new { id = novo.Id }, new
            {
                mensagem = "Usuário cadastrado com sucesso.",
                dados = novo
            });
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] Usuario usuario)
        {
            var atualizado = await _servico.Atualizar(id, usuario);

            if (!atualizado)
                throw new AppException("Usuário não encontrado.", 404);

            return Ok(new { mensagem = "Usuário atualizado com sucesso." });
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _servico.Excluir(id);

            if (!excluido)
                throw new AppException("Usuário não encontrado.", 404);

            return Ok(new { mensagem = "Usuário excluído com sucesso." });
        }
    }
}
