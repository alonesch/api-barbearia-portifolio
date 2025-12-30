namespace BarbeariaPortifolio.API.Modules.Clientes.DTOs;

public class RegistrarNovoClienteDTO
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

}
