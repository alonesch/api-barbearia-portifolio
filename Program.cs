using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// ===================================================
// 🔹 Serviços principais da aplicação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 JWT e Serviços
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthorization();

// ===================================================
// 🔹 Connection String (Railway ou fallback local)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("⚠️ Nenhuma connection string encontrada nas variáveis de ambiente. Usando fallback local.");
    connectionString = "Server=yamabiko.proxy.rlwy.net;Port=15819;Database=railway;User=root;Password=FwIAsbobfoGSFUrfLCSLNrtauWZtPTZN;SslMode=Preferred;";
    Console.ResetColor();
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ===================================================
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

// Exibe Connection String mascarada
var safeConn = connectionString.Contains("Password=")
    ? connectionString.Split("Password=")[0] + "Password=********;"
    : connectionString;

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Connection String carregada com sucesso:");
Console.WriteLine($"   {safeConn}");
Console.ResetColor();

// ===================================================
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

// ===================================================
// ✅ CORS — libera apenas o domínio real da Vercel
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("https://portifolio-gabriel-dun.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ===================================================
// ✅ Aplica migrations automáticas no MySQL (Railway)
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

// ===================================================
// 🧩 Middlewares
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Barbearia Portifolio v1");
    c.RoutePrefix = "swagger"; // acessa em /swagger
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ Necessário no Railway — evita "Invalid Hostname"
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"🚀 Aplicação escutando em: http://0.0.0.0:{port}");
Console.ResetColor();

// ✅ Pós-deploy log
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("🌐 Servidor iniciado com sucesso!");
Console.WriteLine($"📄 Swagger: https://api-barbearia-portifolio-production.up.railway.app/swagger");
Console.WriteLine($"🧩 JSON:    https://api-barbearia-portifolio-production.up.railway.app/swagger/v1/swagger.json");
Console.ResetColor();

app.Run();

