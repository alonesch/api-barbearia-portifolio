using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todos os endpoints por padrao
    public class BarbeiroController : ControllerBase
    {
        private readonly DataContext _context;

        public BarbeiroController(DataContext context)
        {
            _context = context;
        }

        // =======================================
        // LISTAR TODOS OS BARBEIROS
        // =======================================
        [AllowAnonymous] // Disponivel para o site publico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barbeiro>>> GetBarbeiros()
        {
            var barbeiros = await _context.Barbeiros.ToListAsync();
            return Ok(barbeiros);
        }

        // =======================================
        // OBTER BARBEIRO POR ID
        // =======================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Barbeiro>> GetBarbeiro(int id)
        {
            var barbeiro = await _context.Barbeiros.FindAsync(id);
            if (barbeiro == null)
                return NotFound("Barbeiro nao encontrado.");

            return Ok(barbeiro);
        }

        // =======================================
        // CADASTRAR NOVO BARBEIRO
        // =======================================
        [HttpPost]
        public async Task<ActionResult> PostBarbeiro([FromBody] Barbeiro barbeiro)
        {
            if (barbeiro == null)
                return BadRequest("Dados invalidos para cadastro.");

            _context.Barbeiros.Add(barbeiro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBarbeiro), new { id = barbeiro.Id }, barbeiro);
        }

        // =======================================
        // ATUALIZAR BARBEIRO EXISTENTE
        // =======================================
        [HttpPut("{id}")]
        public async Task<ActionResult> PutBarbeiro(int id, [FromBody] Barbeiro barbeiro)
        {
            if (id != barbeiro.Id)
                return BadRequest("O ID informado nao corresponde ao barbeiro.");

            _context.Entry(barbeiro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok("Barbeiro atualizado com sucesso.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Barbeiros.Any(b => b.Id == id))
                    return NotFound("Barbeiro nao encontrado.");
                throw;
            }
        }

        // =======================================
        // EXCLUIR BARBEIRO
        // =======================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarbeiro(int id)
        {
            var barbeiro = await _context.Barbeiros.FindAsync(id);
            if (barbeiro == null)
                return NotFound("Barbeiro nao encontrado.");

            _context.Barbeiros.Remove(barbeiro);
            await _context.SaveChangesAsync();

            return Ok("Barbeiro removido com sucesso.");
        }
    }
}
