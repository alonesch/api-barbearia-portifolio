using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BarbeariaPortifolio.API.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            var connectionString =
                "Server=yamabiko.proxy.rlwy.net;Port=15819;Database=railway;User=root;Password=FwIAsbobfoGSFUrfLCSLNrtauWZtPTZN;SslMode=Preferred;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            return new DataContext(optionsBuilder.Options);
        }
    }
}
