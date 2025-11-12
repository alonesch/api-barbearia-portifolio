using System;

namespace BarbeariaPortifolio.DTOs;

public class ClienteDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
