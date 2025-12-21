using System;

namespace BarbeariaPortifolio.API.Modules.Clientes.DTOs;

public class ClienteDTO
{
    public int UsuarioId { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FotoPerfilUrl { get; set; }

    public string? Cpf {  get; set; }
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

}
