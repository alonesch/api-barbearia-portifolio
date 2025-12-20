using BarbeariaPortifolio.API.Servicos.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BarbeariaPortifolio.API.Servicos
{
    public class EmailServico : IEmailServico
    {
        private readonly IConfiguration _config;

        public EmailServico(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarConfirmacaoEmailAsync(string email, string link)
        {
            var host = _config["EMAIL_HOST"];
            var port = int.Parse(_config["EMAIL_PORT"]!);
            var user = _config["EMAIL_USER"];
            var pass = _config["EMAIL_PASS"];
            var from = _config["EMAIL_FROM"];
            var name = _config["EMAIL_NAME"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass))
            {
                throw new Exception("Configuração SMTP incompleta");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(name, from));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Confirme seu e-mail";

            message.Body = new TextPart("html")
            {
                Text = $@"
                    <h2>Confirmação de e-mail</h2>
                    <p>Para concluir seu cadastro, clique no link abaixo:</p>
                    <p><a href='{link}'>Confirmar e-mail</a></p>
                    <p>Se você não criou essa conta, ignore este e-mail.</p>
                "
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            Console.WriteLine($"[EMAIL] Enviado com sucesso para {email}");
        }
    }
}
