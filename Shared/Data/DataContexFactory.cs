using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BarbeariaPortifolio.API.Shared.Data;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        Console.WriteLine("FACTORY EXECUTANDO");

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        Console.WriteLine($"ENV = {env}");

        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
            ?? (env == "Development"
                ? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_DEV")
                : null);

        Console.WriteLine($"CONNECTION = {connectionString}");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("POSTGRES_CONNECTION não configurada.");

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DataContext(optionsBuilder.Options);
    }
}
