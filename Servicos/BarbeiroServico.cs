using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using BarbeariaPortifolio.API.Models;

namespace BarbeariaPortifolio.API.Servicos
{
    public class BarbeiroServico : IBarbeiroServico
    {
        private readonly IBarbeiroRepositorio _repo;

        public BarbeiroServico(IBarbeiroRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<BarbeiroDTO>> ListarTodos()
        {
            var lista = await _repo.ListarTodos();

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
            var b = await _repo.BuscarPorId(id);
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
            var barbeiro = new Barbeiro
            {
                Nome = dto.Nome,
                Telefone = dto.Telefone
            };

            await _repo.Cadastrar(barbeiro);
            dto.Id = barbeiro.Id;

            return dto;
        }

        public async Task<bool> Atualizar(int id, BarbeiroDTO dto)
        {
            var existente = await _repo.BuscarPorId(id);
            if (existente == null) return false;

            existente.Nome = dto.Nome;
            existente.Telefone = dto.Telefone;

            return await _repo.Atualizar(id, existente);
        }

        public async Task<bool> Excluir(int id)
        {
            return await _repo.Excluir(id);
        }
    }
}
