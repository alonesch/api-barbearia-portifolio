using Microsoft.AspNetCore.Mvc;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ServicoController : ControllerBase
{
	private readonly DataContext _context;



	public  ServicoController(DataContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Servico>>> GetServico()
	{
		var servico = await _context.Servicos.ToListAsync();
		return Ok(servico);
	}

	
	[HttpGet ("{id}")]
	public async Task<ActionResult<Servico>> GetServico(int id)
	{
		var servico = await _context.Servicos.FindAsync(id);

		if (servico == null)
			return NotFound("Serviço não encontrado.");


		return Ok(servico);
	}
	
	
	[HttpPost]
	public async Task<ActionResult<Servico>> PostServico(Servico servico)
	{
		_context.Servicos.Add(servico);
		await _context.SaveChangesAsync();
		return CreatedAtAction(nameof(GetServico), new { id = servico.ID }, servico);
	}
	
	
	[HttpPut("{id}")]
	public async Task<ActionResult<Servico>> PutServico(int id, Servico servico)
	{
		if (id != servico.ID)
			return BadRequest("O ID não corresponde ao serviço.");
		_context.Entry(id).State = EntityState.Modified;
		await _context.SaveChangesAsync();
		return Ok("Serviço atualizado com sucesso.");
	}
	
	
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteServico(int id)
	{ 
		var servico = await _context.Servicos.FindAsync(id);
		if (servico == null)
			return NotFound("Serviço não encontrado");

		_context.Servicos.Remove(servico);
        await _context.SaveChangesAsync();
		return Ok("Serviço criado com sucesso");
	}


	
}
