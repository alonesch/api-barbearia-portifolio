using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AgendamentoController : ControllerBase
    {
        private readonly DataContext _context;

        public AgendamentoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult GetAgendamentos()
        {
            var agendamentos = _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .AsEnumerable()
                .Select(a => new AgendamentoDTO
                {
                    Id = a.Id,
                    Cliente = a.Cliente.Nome,
                    Barbeiro = a.Barbeiro.Nome,
                    DataHora = a.DataHora,
                    Status = a.Status switch
                    {
                        1 => "Pendente",
                        2 => "Confirmado",
                        3 => "Aguardando Pagamento",
                        4 => "Pago",
                        5 => "Cancelado pelo Cliente",
                        6 => "Cancelado pelo Barbeiro",
                        7 => "Finalizado",
                        _ => "Desconhecido"
                    },
                    Observacao = a.AgendamentoServicos
                        .Select(s => s.Observacao)
                        .FirstOrDefault(o => o != null),
                    Servicos = a.AgendamentoServicos
                        .Select(s => new ServicoDTO
                        {
                            NomeServico = s.Servico.NomeServico,
                            Preco = s.Servico.Preco
                        }).ToList()
                })
                .OrderByDescending(a => a.DataHora)
                .ToList();

            return Ok(agendamentos);
        }

        [HttpGet("barbeiro/{usuarioId}")]
        [Authorize(Roles = "Administrador,Barbeiro")]
        public IActionResult GetAgendamentosPorUsuario(int usuarioId)
        {
            var barbeiro = _context.Barbeiros.FirstOrDefault(b => b.UsuarioId == usuarioId);

            if (barbeiro == null)
                return NotFound(new { mensagem = "Barbeiro não encontrado para este usuário." });

            var agendamentos = _context.Agendamentos
                .Where(a => a.BarbeiroId == barbeiro.Id)
                .Include(a => a.Cliente)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(s => s.Servico)
                .AsEnumerable()
                .Select(a => new AgendamentoDTO
                {
                    Id = a.Id,
                    Cliente = a.Cliente.Nome,
                    Barbeiro = barbeiro.Nome,
                    DataHora = a.DataHora,
                    Status = a.Status switch
                    {
                        1 => "Pendente",
                        2 => "Confirmado",
                        3 => "Aguardando Pagamento",
                        4 => "Pago",
                        5 => "Cancelado pelo Cliente",
                        6 => "Cancelado pelo Barbeiro",
                        7 => "Finalizado",
                        _ => "Desconhecido"
                    },
                    Observacao = a.AgendamentoServicos
                        .Select(s => s.Observacao)
                        .FirstOrDefault(o => o != null),
                    Servicos = a.AgendamentoServicos
                        .Select(s => new ServicoDTO
                        {
                            NomeServico = s.Servico.NomeServico,
                            Preco = s.Servico.Preco
                        }).ToList()
                })
                .OrderByDescending(a => a.DataHora)
                .ToList();

            return Ok(agendamentos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Barbeiro")]
        public async Task<ActionResult<Agendamento>> GetAgendamento(int id)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Cliente)
                .Include(a => a.Barbeiro)
                .Include(a => a.AgendamentoServicos)
                    .ThenInclude(asv => asv.Servico)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
                return NotFound("Agendamento não encontrado.");

            return Ok(agendamento);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostAgendamento([FromBody] JsonElement data)
        {
            try
            {
                string nome = data.GetProperty("nome").GetString();
                string cpf = data.TryGetProperty("cpf", out var cpfProp) ? cpfProp.GetString() : null;
                string telefone = data.GetProperty("telefone").GetString();
                int barbeiroId = data.GetProperty("barbeiroId").GetInt32();
                DateTime dataHora = DateTime.Parse(data.GetProperty("dataHora").GetString());

                var agendamentoServicos = data.GetProperty("agendamentoServicos").EnumerateArray()
                    .Select(s => new AgendamentoServico
                    {
                        ServicoId = s.GetProperty("servicoId").GetInt32(),
                        Observacao = s.TryGetProperty("observacao", out var obsProp) ? obsProp.GetString() : null,
                        Quantidade = 1
                    }).ToList();

                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Telefone == telefone);

                if (cliente == null)
                {
                    cliente = new Cliente
                    {
                        Nome = nome,
                        Cpf = cpf,
                        Telefone = telefone,
                        DataCadastro = DateTime.Now
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();
                }

                bool horarioOcupado = await _context.Agendamentos
                    .AnyAsync(a => a.BarbeiroId == barbeiroId && a.DataHora == dataHora);

                if (horarioOcupado)
                    return BadRequest(new { sucesso = false, mensagem = "Horário indisponível para este barbeiro." });

                var novoAgendamento = new Agendamento
                {
                    ClienteId = cliente.ID,
                    BarbeiroId = barbeiroId,
                    DataHora = dataHora,
                    Status = 1,
                    DataRegistro = DateTime.Now,
                    AgendamentoServicos = agendamentoServicos
                };

                _context.Agendamentos.Add(novoAgendamento);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    mensagem = "Agendamento criado com sucesso!",
                    dados = new
                    {
                        novoAgendamento.Id,
                        Cliente = cliente.Nome,
                        DataHora = novoAgendamento.DataHora
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = "Erro ao criar agendamento.", erro = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador,Barbeiro")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] JsonElement data)
        {
            try
            {
                var agendamento = await _context.Agendamentos.FindAsync(id);
                if (agendamento == null)
                    return NotFound("Agendamento não encontrado.");

                int novoStatus = data.GetProperty("status").GetInt32();
                agendamento.Status = novoStatus;

                await _context.SaveChangesAsync();
                return Ok(new { sucesso = true, mensagem = "Status atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = "Erro ao atualizar status.", erro = ex.Message });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgendamento(int id)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.AgendamentoServicos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
                return NotFound("Agendamento não encontrado.");

            _context.AgendamentoServicos.RemoveRange(agendamento.AgendamentoServicos);
            _context.Agendamentos.Remove(agendamento);
            await _context.SaveChangesAsync();

            return Ok("Agendamento removido com sucesso.");
        }
    }
}
