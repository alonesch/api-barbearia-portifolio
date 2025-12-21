using Microsoft.EntityFrameworkCore;

// ===== MODELS POR MÓDULO =====
using BarbeariaPortifolio.API.Modules.Clientes.Models;
using BarbeariaPortifolio.API.Modules.Barbeiros.Models;
using BarbeariaPortifolio.API.Modules.Services.Models;
using BarbeariaPortifolio.API.Modules.Agendamentos.Models;
using BarbeariaPortifolio.API.Modules.Agendamentos.Models.Enums;
using BarbeariaPortifolio.API.Modules.Usuarios.Models;
using BarbeariaPortifolio.API.Modules.Disponibilidades.Models;
using BarbeariaPortifolio.API.Modules.Auth.Models;


namespace BarbeariaPortifolio.API.Shared.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Barbeiro> Barbeiros => Set<Barbeiro>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<AgendamentoServico> AgendamentoServicos => Set<AgendamentoServico>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Disponibilidade> Disponibilidades { get; set; } = null!;
    public DbSet<EmailConfirmacaoToken> EmailConfirmacaoTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgendamentoServico>()
            .HasKey(x => new { x.AgendamentoId, x.ServicoId });

        modelBuilder.Entity<Cliente>()
            .Property(c => c.DataCadastro)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Agendamento>()
            .Property(a => a.DataRegistro)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Agendamento>()
            .Property(a => a.Status)
            .HasDefaultValue(StatusAgendamento.Pendente);

        modelBuilder.Entity<Barbeiro>()
            .HasOne(b => b.Usuario)
            .WithOne()
            .HasForeignKey<Barbeiro>(b => b.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Disponibilidade>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Data).IsRequired();
            entity.Property(x => x.Hora).IsRequired();

            entity.Property(x => x.Ativo).HasDefaultValue(true);
            entity.Property(x => x.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(x => x.Barbeiro)
                .WithMany()
                .HasForeignKey(x => x.BarbeiroId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EmailConfirmacaoToken>()
            .HasOne(e => e.Usuario)
            .WithMany()
            .HasForeignKey(e => e.UsuarioId);

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Disponibilidade)
            .WithMany(d => d.Agendamentos)
            .HasForeignKey(a => a.DisponibilidadeId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Agendamento>()
            .HasIndex(a => a.DisponibilidadeId)
            .IsUnique();
    }
}
