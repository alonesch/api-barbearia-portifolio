using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Exceptions;

namespace BarbeariaPortifolio.API.Servicos
{
    public class UsuarioServico : IUsuarioServico
    {
        private readonly IUsuarioRepositorio _repositorio;

        public UsuarioServico(IUsuarioRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        // Futuro:
        // public async Task<IEnumerable<Usuario>> ListarTodos()
        //     => await _repositorio.ListarTodos();

        public async Task<Usuario?> BuscarPorId(int id)
            => await _repositorio.BuscarPorId(id);

        public async Task<Usuario> Cadastrar(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
                throw new AppException("Nome do usuário é obrigatório.", 400);

            if (string.IsNullOrWhiteSpace(usuario.SenhaHash))
                throw new AppException("Senha é obrigatória.", 400);



            return await _repositorio.Cadastrar(usuario);
        }

        public async Task<bool> Atualizar(int id, Usuario usuario)
        {
            if (id != usuario.Id)
                throw new AppException("O ID informado na rota não coincide com o ID do usuário.", 400);

            var existente = await _repositorio.BuscarPorId(id);
            if (existente == null)
                throw new AppException("Usuario não encontrado", 404);

            existente.NomeUsuario = usuario.NomeUsuario;
            existente.NomeCompleto = usuario.NomeCompleto;
            existente.Cargo = usuario.Cargo;
            existente.Role = usuario.Role;
            existente.Ativo = usuario.Ativo;

            if (!string.IsNullOrWhiteSpace(usuario.SenhaHash))
                existente.SenhaHash = usuario.SenhaHash;

            return await _repositorio.Atualizar(id, existente);
        }

        public async Task<bool> Excluir(int id)
        {
            var existente = await _repositorio.BuscarPorId(id);
            if (existente == null)
                throw new AppException("Usuario não encontrado", 404);

            return await _repositorio.Excluir(id);

        }
    }
}
