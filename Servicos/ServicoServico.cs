using BarbeariaPortifolio.API.Exceptions;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos
{
    public class ServicoServico : IServicoServico
    {
        private readonly IServicoRepositorio _repositorio;

        public ServicoServico(IServicoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<ServicoDTO>> ListarTodos()
        {
            var lista = await _repositorio.ListarTodos();

            return lista.Select(s => new ServicoDTO
            {
                Id = s.Id,
                NomeServico = s.NomeServico,
                Preco = s.Preco,
                DuracaoServico = s.DuracaoServico

            });
        }

        public async Task<ServicoDTO?> BuscarPorId(int id)
        {
            var servico = await _repositorio.BuscarPorId(id);
            if (servico == null) return null;

            return new ServicoDTO
            {
                Id = servico.Id,
                NomeServico = servico.NomeServico,
                Preco = servico.Preco
            };
        }

        public async Task<ServicoDTO> Cadastrar(ServicoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NomeServico))
                throw new AppException("O nome do serviço é obrigatório.", 400);

            if (dto.Preco <= 0)
                throw new AppException("O preço do serviço deve ser maior que zero.", 400);

            var servico = new Servico
            {
                NomeServico = dto.NomeServico,
                Preco = dto.Preco
            };

            await _repositorio.Cadastrar(servico);

            dto.Id = servico.Id;
            return dto;
        }

        public async Task<bool> Atualizar(int id, ServicoDTO dto)
        {
            var existente = await _repositorio.BuscarPorId(id);
            if (existente == null)
                return false;

            existente.NomeServico = dto.NomeServico;
            existente.Preco = dto.Preco;

            return await _repositorio.Atualizar(id, existente);
        }

        public async Task<bool> Excluir(int id)
        {
            return await _repositorio.Excluir(id);
        }
    }
}
