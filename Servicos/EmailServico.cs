using System.Net;
using System.Net.Mail;
using BarbeariaPortifolio.API.Servicos.Interfaces;

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

            var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(from!, name),
                Subject = "Confirme seu e-mail",
                Body = $@"
                    <h2>Confirmação de e-mail</h2>
                    <p>Para concluir seu cadastro, clique no link abaixo:</p>
                    <p><a href='{link}'>Confirmar e-mail</a></p>
                    <p>Se você não criou essa conta, ignore este e-mail.</p>
                ",
                IsBodyHtml = true
            };

            mail.To.Add(email);

            await smtp.SendMailAsync(mail);
        }
    }
}
