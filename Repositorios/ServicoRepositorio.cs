using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaPortifolio.API.Repositorios

{
    public class ServicoRepositorio : IServicoRepositorio
    {
        private readonly DataContext _banco;


        public ServicoRepositorio(DataContext banco)
        {
            _banco = banco;
        }


        public async Task<IEnumerable<Servico>> ListarTodos()
            => await _banco.Servicos.ToListAsync();

        public async Task<Servico?> BuscarPorId(int id)
            => await _banco.Servicos.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

        public async Task<Servico> Cadastrar(Servico servico)
        {
            _banco.Servicos.Add(servico);
            await _banco.SaveChangesAsync();
            return servico;
        }

        public async Task<bool> Atualizar(int id, Servico servico)
        {
            if (id != servico.Id) return false;

            _banco.Entry(servico).State = EntityState.Modified;
            await _banco.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Excluir(int id)
        {
            var servico = await BuscarPorId(id);
            if (servico == null) return false;
            
            _banco.Servicos.Remove(servico);
            await _banco.SaveChangesAsync();
            return true;
        }

    }

}
