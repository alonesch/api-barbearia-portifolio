using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;

namespace BarbeariaPortifolio.API.Servicos
{
    public class AgendamentoServico : IAgendamentoServico
    {
        private readonly IAgendamentoRepositorio _repositorio;

        public AgendamentoServico(IAgendamentoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarTodos()
        {
            var agendamentos = await _repositorio.ListarTodos();

            return agendamentos.Select(a => new AgendamentoDTO
            {
                Id = a.Id,
                Cliente = a.Cliente.Nome,
                Barbeiro = a.Barbeiro.Nome,
                DataHora = a.DataHora,
                Status = a.Status,
                Servicos = a.AgendamentoServicos.Select(s => new ServicoDTO

                {
                    NomeServico = s.Servico.NomeServico,
                    Preco = s.Servico.Preco
                }).ToList()
            });
        }

        public async Task<AgendamentoDTO?> BuscarPorId(int id)
        {
            var a = await _repositorio.BuscarPorId(id);
            if (a == null) return null;

            return new AgendamentoDTO
            {
                Id = a.Id,
                Cliente = a.Cliente.Nome,
                Barbeiro = a.Barbeiro.Nome,
                DataHora = a.DataHora,
                Status = a.Status,
                Servicos = a.AgendamentoServicos.Select(s => new ServicoDTO
                {
                    NomeServico = s.Servico.NomeServico,
                    Preco = s.Servico.Preco,
                    Observacao = s.Observacao
                }).ToList()
            };

        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiro(int barbeiroId)
        {
            var agendamentos = await _repositorio.ListarPorBarbeiro(barbeiroId);

            return agendamentos.Select(a => new AgendamentoDTO
            {
                Id = a.Id,
                Cliente = a.Cliente.Nome,
                Barbeiro = a.Barbeiro.Nome,
                DataHora = a.DataHora,
                Status = a.Status,
                Servicos = a.AgendamentoServicos.Select(s => new ServicoDTO
                {
                    NomeServico = s.Servico.NomeServico,
                    Preco = s.Servico.Preco
                }).ToList()
            });
        }

        public async Task<AgendamentoDTO> Cadastrar(AgendamentoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Cliente))
                throw new Exception("O cliente é obrigatório.");

            if (dto.DataHora == default)
                throw new Exception("A data e hora do agendamento são obrigatórias.");

            var agendamento = new Agendamento
            {
                ClienteId = 0, // será buscado no repositório
                BarbeiroId = 0, // idem
                DataHora = dto.DataHora,
                Status = dto.Status,
                
            };

            await _repositorio.Cadastrar(agendamento);
            dto.Id = agendamento.Id;
            return dto;
        }

        public async Task<bool> Atualizar(int id, AgendamentoDTO dto)
        {
            var agendamento = await _repositorio.BuscarPorId(id);
            if (agendamento == null) return false;

            agendamento.DataHora = dto.DataHora;
            agendamento.Status = dto.Status;

            return await _repositorio.Atualizar(id, agendamento);
        }

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);
    }
}
