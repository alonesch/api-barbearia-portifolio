using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos
{
    public class BarbeiroServico : IBarbeiroServico
    {
        private readonly IBarbeiroRepositorio _repositorio;

        public BarbeiroServico(IBarbeiroRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<BarbeiroDTO>> ListarTodos()
            => await _repositorio.ListarTodos();

        public async Task<Barbeiro?> BuscarPorId(int id)
            => await _repositorio.BuscarPorId(id);

        public async Task<Barbeiro> Cadastrar(Barbeiro barbeiro)
        {
            if (string.IsNullOrWhiteSpace(barbeiro.Nome))
                throw new Exception("O nome do barbeiro é obrigatório.");

            if (string.IsNullOrWhiteSpace(barbeiro.Telefone))
                throw new Exception("O telefone do barbeiro é obrigatório.");

            return await _repositorio.Cadastrar(barbeiro);
        }

        public async Task<bool> Atualizar(int id, Barbeiro barbeiro)
            => await _repositorio.Atualizar(id, barbeiro);

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);
    }
}
