using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// 🔹 JWT e Serviços
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// ✅ Lê a Connection String primeiro da variável de ambiente (Railway), senão usa a local
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("⚠️ Nenhuma connection string encontrada nas variáveis de ambiente.");
    Console.WriteLine("Usando fallback local.");
    connectionString = "Server=yamabiko.proxy.rlwy.net;Port=15819;Database=railway;User=root;Password=FwIAsbobfoGSFUrfLCSLNrtauWZtPTZN;SslMode=Preferred;";
    Console.ResetColor();
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🔹 JWT Key
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var keyValue = builder.Configuration["Jwt:Key"] ?? string.Empty;

if (string.IsNullOrWhiteSpace(keyValue))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ ERRO: Nenhuma chave JWT foi encontrada!");
    Console.WriteLine("   Defina a variável de ambiente Jwt__Key antes de iniciar o servidor.");
    Console.ResetColor();
    Environment.Exit(1);
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"✅ Ambiente atual: {builder.Environment.EnvironmentName}");
Console.WriteLine($"✅ JWT Key carregada ({keyValue.Length} caracteres)");
Console.ResetColor();

// 🔹 Mostra Connection String (sem a senha)
var safeConn = connectionString.Contains("Password=")
    ? connectionString.Split("Password=")[0] + "Password=********;"
    : connectionString;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Connection String carregada com sucesso:");
Console.WriteLine($"   {safeConn}");
Console.ResetColor();

// 🔹 Configuração JWT
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ Aplica Migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.Database.Migrate();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Banco de dados atualizado com sucesso!");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ Erro ao atualizar o banco: {ex.Message}");
        Console.ResetColor();
    }
}

app.Run();
