using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;




namespace BarbeariaPortifolio.API.Servicos
{
    public class ServicoServico : IServicoServico
    {
        private readonly IServicoRepositorio _repositorio;

        public ServicoServico(IServicoRepositorio repositorio)
        {
               _repositorio = repositorio;  
        }

        public async Task<IEnumerable<Servico>> ListarTodos()
            => await _repositorio.ListarTodos();

        public async Task<Servico?> BuscarPorId(int id)
            => await _repositorio.BuscarPorId(id);

        public async Task<Servico> Cadastrar(Servico servico)
        {     
            if (string.IsNullOrWhiteSpace(servico.NomeServico))
                throw new Exception("O nome do serviço é obrigatório.");

            if (servico.Preco <= 0)
                throw new Exception("O preço do serviço deve ser maior que zero.");
            
            return await _repositorio.Cadastrar(servico);

        }
        
        public async Task<bool> Atualizar(int id, Servico servico)
            => await _repositorio.Atualizar(id, servico);

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);
    }
}

