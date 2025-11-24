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
                "Server=mysqldbbarberprd-production.up.railway.app;Port=3306;Database=railway;User Id=root;Password=FwIAsbobfoGSFUrfLCSLNrtauWZtPTZN;SslMode=Preferred;AllowPublicKeyRetrieval=True;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            return new DataContext(optionsBuilder.Options);
        }
    }
}
