namespace BarbeariaPortifolio.API.Modules.Auth.DTOs;

/// <summary>
/// DTO usado para solicitar o reenvio do e-mail de confirmação.
/// Entrada mínima: apenas o e-mail.
/// </summary>
public class ReenviarConfirmacaoEmailDto
{
    public string Email { get; set; } = string.Empty;
}
