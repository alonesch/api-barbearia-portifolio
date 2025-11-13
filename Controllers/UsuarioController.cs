using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListarTodos()
            => Ok(await _servico.ListarTodos());

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var usuario = await _servico.BuscarPorId(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        
        [HttpPost]
        public async Task<IActionResult> Cadastrar(Usuario usuario)
        {
            var novo = await _servico.Cadastrar(usuario);
            return CreatedAtAction(nameof(BuscarPorId), new { id = novo.Id }, novo);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Usuario usuario)
        {
            var atualizado = await _servico.Atualizar(id, usuario);
            return atualizado
                ? NoContent()
                : BadRequest("Ids não coincidem.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _servico.Excluir(id);
            return excluido
                ? NoContent()
                : NotFound();
        }
    }
}
