using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BarbeariaPortifolio.API.Servicos
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _repositorio;

        public UsuarioServico(IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<UsuarioDTO>> ListarTodos()
        {
            var usuarios = await _repositorio.ListarTodos();

            return usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                NomeUsuario = u.NomeUsuario,
                Role = u.Role,
                BarbeiroId = u.Barbeiro?.Id
            });
        }


        public async Task<Usuario?> BuscarPorId(int id)
            => await _repositorio.BuscarPorId(id);

        public async Task<Usuario> Cadastrar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
                throw new Exception("Nome do usuário é obrigatório.");

            if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
                throw new Exception("A senha é obrigatória.");

            return await _repositorio.Cadastrar(usuario);
        }

        public async Task<bool> Atualizar(int id, Usuario usuario)
            => await _repositorio.Atualizar(id, usuario);

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);
    }
}
