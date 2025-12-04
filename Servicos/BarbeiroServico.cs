using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Exceptions;
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
        {
            var lista = await _repositorio.ListarTodos();

            return lista.Select(b => new BarbeiroDTO
            {
                Id = b.Id,
                Nome = b.Nome,
                Telefone = b.Telefone,
                Usuario = b.Usuario?.NomeUsuario,
                Agendamentos = b.Agendamentos?.Select(a => new AgendamentoDTO
                {
                    Id = a.Id,
                    Nome = a.Cliente.Nome,
                    Cpf = a.Cliente.Cpf,
                    Telefone = a.Cliente.Telefone,
                    BarbeiroId = a.BarbeiroId,
                    DataHora = a.DataHora,
                    Status = a.Status,
                    Observacao = a.Observacao,
                    AgendamentoServicos = a.AgendamentoServicos.Select(s => new AgendamentoServicoDTO
                    {
                        ServicoId = s.ServicoId,
                        Observacao = s.Observacao
                    }).ToList()
                }).ToList()
            });
        }

        public async Task<BarbeiroDTO?> BuscarPorId(int id)
        {
            var b = await _repositorio.BuscarPorId(id);
            if (b == null) return null;

            return new BarbeiroDTO
            {
                Id = b.Id,
                Nome = b.Nome,
                Telefone = b.Telefone,
                Usuario = b.Usuario?.NomeUsuario,
                Agendamentos = b.Agendamentos?.Select(a => new AgendamentoDTO
                {
                    Id = a.Id,
                    Nome = a.Cliente.Nome,
                    Cpf = a.Cliente.Cpf,
                    Telefone = a.Cliente.Telefone,
                    BarbeiroId = a.BarbeiroId,
                    DataHora = a.DataHora,
                    Status = a.Status,
                    Observacao = a.Observacao,
                    AgendamentoServicos = a.AgendamentoServicos.Select(s => new AgendamentoServicoDTO
                    {
                        ServicoId = s.ServicoId,
                        Observacao = s.Observacao
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<BarbeiroDTO> Cadastrar(BarbeiroDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new AppException("O nome do barbeiro é obrigatório.", 400);
            
            if (string.IsNullOrWhiteSpace(dto.Telefone))
                throw new AppException("O telefone do barbeiro é obrigatório.", 400);

            var barbeiro = new Barbeiro
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone
            };

            await _repositorio.Cadastrar(barbeiro);
            dto.Id = barbeiro.Id;

            return dto;
        }

        public async Task<bool> Atualizar(int id, BarbeiroDTO dto)
        {
            var existente = await _repositorio.BuscarPorId(id);
            if (existente == null) return false;

            if(string.IsNullOrWhiteSpace(dto.Nome))
                throw new AppException("O nome do barbeiro é obrigatório.", 400);

            if(string.IsNullOrWhiteSpace(dto.Telefone))
                throw new AppException("O telefone do barbeiro é obrigatório.", 400);


            existente.Nome = dto.Nome;
            existente.Telefone = dto.Telefone;

            return await _repositorio.Atualizar(id, existente);
        }

        public async Task<bool> Excluir(int id)
        {
            return await _repositorio.Excluir(id);
        }
    }
}
