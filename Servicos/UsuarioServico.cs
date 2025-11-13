using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.DTOs;

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
            => await _repositorio.ListarTodos();

        public async Task<Usuario?> BuscarPorId(int id)
            => await _repositorio.BuscarPorId(id);

        public async Task<Usuario> Cadastrar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
                throw new Exception("Nome do usuário é obrigatório");

            if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
                throw new Exception("Senha é obrigatória");

            return await _repositorio.Cadastrar(usuario);
        }

        public async Task<bool> Atualizar(int id, Usuario usuario)
            => await _repositorio.Atualizar(id, usuario);

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);
    }
}
