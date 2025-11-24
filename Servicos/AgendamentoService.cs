using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using BarbeariaPortifolio.DTOs;
using BarbeariaPortifolio.API.Exceptions;

namespace BarbeariaPortifolio.API.Servicos
{
    public class AgendamentoService : IAgendamentoServico
    {
        private readonly IAgendamentoRepositorio _repositorio;

        public AgendamentoService(IAgendamentoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarTodos()
        {
            var agendamentos = await _repositorio.ListarTodos();
            return agendamentos.Select(MapToDto);
        }

        public async Task<AgendamentoDTO?> BuscarPorId(int id)
        {
            var agendamento = await _repositorio.BuscarPorId(id);
            return agendamento == null ? null : MapToDto(agendamento);
        }

        public async Task<IEnumerable<AgendamentoDTO>> ListarPorBarbeiro(int barbeiroId)
        {
            var agendamentos = await _repositorio.ListarPorBarbeiro(barbeiroId);
            return agendamentos.Select(MapToDto);
        }

        public async Task<AgendamentoDTO> Cadastrar(AgendamentoDTO dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new Exception("O nome do cliente é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Telefone))
                throw new Exception("O telefone do cliente é obrigatório.");

            if (dto.BarbeiroId <= 0)
                throw new Exception("Barbeiro inválido.");

            if (dto.DataHora == default)
                throw new Exception("A data e hora do agendamento são obrigatórias.");

            if (dto.AgendamentoServicos == null || dto.AgendamentoServicos.Count == 0)
                throw new Exception("Selecione pelo menos um serviço.");

            var conflito = await _repositorio.ChecarHorarios(dto.BarbeiroId, dto.DataHora);


            if (conflito)
                throw new AppException("Horário já reservado!", 409);

            var cliente = await _repositorio.BuscarOuCriarCliente(dto.Nome, dto.Cpf, dto.Telefone);


            var agendamento = new Agendamento
            {
                ClienteId = cliente.Id,
                BarbeiroId = dto.BarbeiroId,
                DataHora = dto.DataHora,
                Status = dto.Status > 0 ? dto.Status : 1,
                Observacao = dto.Observacao
            };

            await _repositorio.Cadastrar(agendamento);


            foreach (var s in dto.AgendamentoServicos)
            {
                var item = new AgendamentoServico
                {
                    AgendamentoId = agendamento.Id,
                    ServicoId = s.ServicoId,
                    Observacao = s.Observacao
                };

                await _repositorio.CadastrarAgendamentoServico(item);
            }


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

            return await _repositorio.Atualizar(agendamento);
        }

        public async Task<bool> AlterarStatus(int id, int novoStatus)
        {
            var agendamento = await _repositorio.BuscarStatusId(id);

            if (agendamento == null)
                return false;

            agendamento.Status = novoStatus;

            await _repositorio.AlterarStatus(agendamento);
            return true;
        }

        public async Task<bool> Excluir(int id)
            => await _repositorio.Excluir(id);


        private static AgendamentoDTO MapToDto(Agendamento a)
        {
            return new AgendamentoDTO
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
            };
        }
    }
}
