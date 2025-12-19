
//namespace BarbeariaPortifolio.API.Servico.Interfaces;
using BarbeariaPortifolio.API.Models;
using System.Threading.Tasks;

public interface IEmailServico
{
    Task EnviarConfirmacaoEmailAsync(string email, string link);
}
