using BarbeariaPortifolio.API.Modules.Auth.Models;
using System.Threading.Tasks;


namespace BarbeariaPortifolio.API.Modules.Auth.Services.Interfaces;
public interface IEmailServico
{
    Task EnviarConfirmacaoEmailAsync(string email, string link);
}
