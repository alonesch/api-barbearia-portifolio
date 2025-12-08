using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Servicos.Interfaces;

namespace BarbeariaPortifolio.API.Servicos
{
    public class EmailServico : IEmailServico
    {
        public Task EnviarEmailConfirmacaoAsync(Usuario usuario, string token)
        {
            var link = $"https://seusite.com/api/auth/confirmar-email?token={token}";

            Console.WriteLine("=======================================");
            Console.WriteLine(" MOCK DE ENVIO DE EMAIL ");
            Console.WriteLine($" Para: {usuario.Email}");
            Console.WriteLine($" Usuário: {usuario.NomeUsuario}");
            Console.WriteLine($" Link: {link}");
            Console.WriteLine("=======================================");

            return Task.CompletedTask;
        }
    }
}
