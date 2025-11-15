using Microsoft.EntityFrameworkCore;
using BarbeariaPortifolio.API.Models;
using System;

namespace BarbeariaPortifolio.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Barbeiro> Barbeiros => Set<Barbeiro>();
        public DbSet<Servico> Servicos => Set<Servico>();
        public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
        public DbSet<AgendamentoServico> AgendamentoServicos => Set<AgendamentoServico>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgendamentoServico>()
                .HasKey(x => new { x.AgendamentoId, x.ServicoId });

            modelBuilder.Entity<Cliente>()
                .Property(c => c.DataCadastro)
                .ValueGeneratedOnAdd()
                .HasDefaultValue(DateTime.UtcNow);

            modelBuilder.Entity<Agendamento>()
                .Property(a => a.DataRegistro)
                .ValueGeneratedOnAdd()
                .HasDefaultValue(DateTime.UtcNow);

            modelBuilder.Entity<Agendamento>()
                .Property(a => a.Status)
                .HasDefaultValue(1);

            modelBuilder.Entity<Barbeiro>()
                .HasOne(b =>  b.Usuario)
                .WithOne()
                .HasForeignKey<Barbeiro>(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
