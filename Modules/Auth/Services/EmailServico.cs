using BarbeariaPortifolio.API.Modules.Auth.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BarbeariaPortifolio.API.Modules.Auth.Services;

public class EmailServico : IEmailServico
{
    private readonly IConfiguration _config;

    public EmailServico(IConfiguration config)
    {
        _config = config;
    }

    public async Task EnviarConfirmacaoEmailAsync(
        string email,
        string nomeUsuario,
        string link
    )
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

        // =========================
        // CARREGAR TEMPLATE HTML
        // =========================
        var templatePath = Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            "email-confirmacao.html"
        );

        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Template de e-mail não encontrado", templatePath);

        var htmlTemplate = await File.ReadAllTextAsync(templatePath);

        // =========================
        // MAPA DE TAGS (CONTRATO)
        // =========================
        var tags = new Dictionary<string, string>
        {
            ["<nome-usuario>"] = nomeUsuario,
            ["<link-confirmacao>"] = link,
            ["<ano-atual>"] = DateTime.Now.Year.ToString()
        };

        foreach (var tag in tags)
        {
            htmlTemplate = htmlTemplate.Replace(tag.Key, tag.Value);
        }

        // =========================
        // MONTAR E-MAIL
        // =========================
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(name, from));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Confirme seu e-mail";

        message.Body = new TextPart("html")
        {
            Text = htmlTemplate
        };

        // =========================
        // ENVIAR
        // =========================
        using var client = new SmtpClient();

        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(user, pass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        Console.WriteLine($"[EMAIL] Confirmação enviada para {email}");
    }
}
