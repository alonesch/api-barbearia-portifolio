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
                ClienteId = a.ClienteId,
                BarbeiroId = a.BarbeiroId,
                DataHora = a.DataHora,
                Status = a.Status,
                Observacao = a.Observacao,
                ServicosIds = a.AgendamentoServicos?
                    .Select(s => s.ServicoId)
                    .ToList() ?? new List<int>()
            });
        }

        public async Task<AgendamentoDTO?> BuscarPorId(int id)
        {
            var a = await _repositorio.BuscarPorId(id);
            if (a == null) return null;

            return new AgendamentoDTO
            {
                Id = a.Id,
                ClienteId = a.ClienteId,
                BarbeiroId = a.BarbeiroId,
                DataHora = a.DataHora,
                Status = a.Status,
                Observacao = a.Observacao,
                ServicosIds = a.AgendamentoServicos?
                    .Select(s => s.ServicoId)
                    .ToList() ?? new List<int>()
            };
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiro(int barbeiroId)
        {
            var agendamentos = await _repositorio.ListarPorBarbeiro(barbeiroId);

            return agendamentos.Select(a => new AgendamentoDTO
            {
                Id = a.Id,
                ClienteId = a.ClienteId,
                BarbeiroId = a.BarbeiroId,
                DataHora = a.DataHora,
                Status = a.Status,
                Observacao = a.Observacao,
                ServicosIds = a.AgendamentoServicos?
                    .Select(s => s.ServicoId)
                    .ToList() ?? new List<int>()
            });
        }

        public async Task<AgendamentoDTO> Cadastrar(AgendamentoDTO dto)
        {
            if (dto.ClienteId <= 0)
                throw new Exception("ClienteId é obrigatório.");

            if (dto.BarbeiroId <= 0)
                throw new Exception("BarbeiroId é obrigatório.");

            if (dto.DataHora == default)
                throw new Exception("A data e hora do agendamento são obrigatórias.");

            if (dto.ServicosIds == null || dto.ServicosIds.Count == 0)
                throw new Exception("Ao menos um serviço é obrigatório.");

            var agendamento = new Agendamento
            {
                ClienteId = dto.ClienteId,
                BarbeiroId = dto.BarbeiroId,
                DataHora = dto.DataHora,
                Status = dto.Status,
                Observacao = dto.Observacao,
                DataRegistro = DateTime.UtcNow,
                AgendamentoServicos = dto.ServicosIds.Select(id =>
                    new BarbeariaPortifolio.API.Models.AgendamentoServico
                    {
                        ServicoId = id
                    }).ToList()
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
            agendamento.Observacao = dto.Observacao;

            // se quiser atualizar serviços depois, faz aqui
            return await _repositorio.Atualizar(id, agendamento);
        }

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);
    }
}
