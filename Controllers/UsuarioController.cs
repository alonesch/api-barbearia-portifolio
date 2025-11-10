using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using System.Linq;

namespace BarbeariaPortifolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : ControllerBase
    {
        private readonly DataContext _context;

        public UsuarioController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CriarUsuario([FromBody] Usuario novoUsuario)
        {
            if (novoUsuario == null || string.IsNullOrWhiteSpace(novoUsuario.NomeUsuario) || string.IsNullOrWhiteSpace(novoUsuario.Senha))
                return BadRequest(new { mensagem = "Usuário e senha são obrigatórios." });

            if (_context.Usuarios.Any(u => u.NomeUsuario == novoUsuario.NomeUsuario))
                return Conflict(new { mensagem = "Usuário já existe." });

            novoUsuario.Senha = BCrypt.Net.BCrypt.HashPassword(novoUsuario.Senha);

            _context.Usuarios.Add(novoUsuario);
            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Usuário criado com sucesso.",
                usuario = new
                {
                    novoUsuario.Id,
                    novoUsuario.NomeUsuario,
                    novoUsuario.NomeCompleto,
                    novoUsuario.Cargo,
                    novoUsuario.Ativo
                }
            });
        }

        [HttpGet("{id}")]
        public IActionResult BuscarUsuario(int id)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Barbeiro)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return Ok(new
            {
                usuario.Id,
                usuario.NomeUsuario,
                usuario.NomeCompleto,
                usuario.Cargo,
                usuario.Ativo,
                barbeiro = usuario.Barbeiro != null ? new
                {
                    usuario.Barbeiro.Id,
                    usuario.Barbeiro.Nome,
                    usuario.Barbeiro.Telefone
                } : null
            });
        }

        [HttpPost("{usuarioId}/vincular-barbeiro/{barbeiroId}")]
        public IActionResult VincularBarbeiro(int usuarioId, int barbeiroId)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
            var barbeiro = _context.Barbeiros.FirstOrDefault(b => b.Id == barbeiroId);

            if (usuario == null) return NotFound(new { mensagem = "Usuário não encontrado." });
            if (barbeiro == null) return NotFound(new { mensagem = "Barbeiro não encontrado." });

            var barbeiroAntigo = _context.Barbeiros.FirstOrDefault(b => b.UsuarioId == usuario.Id);
            if (barbeiroAntigo != null)
            {
                barbeiroAntigo.UsuarioId = null;
                _context.Barbeiros.Update(barbeiroAntigo);
            }

            usuario.BarbeiroId = barbeiro.Id;
            barbeiro.UsuarioId = usuario.Id;

            _context.Usuarios.Update(usuario);
            _context.Barbeiros.Update(barbeiro);
            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Vínculo realizado com sucesso.",
                usuario = new
                {
                    usuario.Id,
                    usuario.NomeUsuario,
                    usuario.NomeCompleto,
                    usuario.Cargo,
                    usuario.BarbeiroId
                },
                barbeiro = new
                {
                    barbeiro.Id,
                    barbeiro.Nome,
                    barbeiro.Telefone,
                    barbeiro.UsuarioId
                }
            });
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarUsuario(int id, [FromBody] Usuario dadosAtualizados)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Barbeiro)
                .FirstOrDefault(u => u.Id == id);

            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            usuario.NomeCompleto = dadosAtualizados.NomeCompleto ?? usuario.NomeCompleto;
            usuario.Cargo = dadosAtualizados.Cargo ?? usuario.Cargo;
            usuario.Ativo = dadosAtualizados.Ativo;

            if (!string.IsNullOrWhiteSpace(dadosAtualizados.Senha))
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(dadosAtualizados.Senha);

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Usuário atualizado com sucesso.",
                usuario = new
                {
                    usuario.Id,
                    usuario.NomeUsuario,
                    usuario.NomeCompleto,
                    usuario.Cargo,
                    usuario.Ativo
                }
            });
        }
    }
}
