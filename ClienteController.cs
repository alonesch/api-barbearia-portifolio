using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BarbeariaPortfolio.API.Controllers;


[ApiController]
[Route("api/[Controller]")]
public class ClienteController : ControllerBase
{
    private readonly DataContext _context;

    public ClienteController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetCliente()
    {
        var cliente = await _context.Clientes.ToListAsync();
        return Ok(cliente);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync();

        if (cliente == null)
            return NotFound("Cliente não encontrado.");

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCliente), new { id = cliente.ID }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutClient(int id, Cliente cliente)
    {
        if (id != cliente.ID)
            return BadRequest("O ID informado não corresponde ao cliente!");

        _context.Entry(cliente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound("Cliente não encontrado.");

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();

        return Ok("Cliente removido com sucesso.");
    }
}