namespace BarbeariaPortifolio.DTOs
{
    public class BarbeiroDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        // Dados do usuário vinculado (opcional)
        public UsuarioDTO? Usuario { get; set; }

        // Lista de agendamentos do barbeiro
        public List<AgendamentoDTO> Agendamentos { get; set; } = new();
    }
}
