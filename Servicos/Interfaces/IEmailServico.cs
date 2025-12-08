
//namespace BarbeariaPortifolio.API.Servico.Interfaces;
using BarbeariaPortifolio.API.Models;

public interface IEmailServico
{
    Task EnviarEmailConfirmacaoAsync(Usuario usuario, string token);
}
