using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Middleware;
using BarbeariaPortifolio.API.Repositorios;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);



// =======================================================================
// SWAGGER
// =======================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthorization();

// =======================================================================
// DATABASE
// =======================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("⚠️ Nenhuma connection string encontrada. Usando fallback...");
    connectionString =
        "Server=yamabiko.proxy.rlwy.net;Port=15819;Database=railway;User=root;Password=FwIAsbobfoGSFUrfLCSLNrtauWZtPTZN;";
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// =======================================================================
// JWT
// =======================================================================
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var keyValue = builder.Configuration["Jwt:Key"] ?? string.Empty;

if (string.IsNullOrWhiteSpace(keyValue))
{
    Console.WriteLine("❌ Nenhuma chave JWT encontrada!");
    Environment.Exit(1);
}

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
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


// =======================================================================
// CORS DEFINITIVO (PROFISSIONAL)
// =======================================================================
builder.Services.AddCors(options =>
{
    // PRODUÇÃO – apenas domínio oficial
    options.AddPolicy("prd", policy =>
        policy.WithOrigins("https://barbearia-gabriel-port.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
    );

    // DESENVOLVIMENTO / PREVIEW – frontend dev e localhost liberados
    options.AddPolicy("dev", policy =>
        policy.WithOrigins(
                "https://dev-barbearia-gabriel-port.vercel.app",
                "http://localhost:5173",
                "http://localhost:3000"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
    );
});


// =======================================================================
// DEPENDENCY INJECTION
// =======================================================================
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IClienteServico, ClienteServico>();
builder.Services.AddScoped<IServicoRepositorio, ServicoRepositorio>();
builder.Services.AddScoped<IServicoServico, ServicoServico>();
builder.Services.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();
builder.Services.AddScoped<IAgendamentoServico, AgendamentoService>();
builder.Services.AddScoped<IBarbeiroRepositorio, BarbeiroRepositorio>();
builder.Services.AddScoped<IBarbeiroServico, BarbeiroServico>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IUsuarioServico, UsuarioServico>();
builder.Services.AddScoped<IAuthServico, AuthServico>();
builder.Services.AddScoped<IRefreshTokenRepositorio, RefreshTokenRepositorio>();


// =======================================================================
// KESTREL (PORTA RAILWAY)
// =======================================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

builder.WebHost.UseSetting("AllowedHosts", "*");


// =======================================================================
// BUILD APP
// =======================================================================
var app = builder.Build();

// =======================================================================
// MIGRATIONS AUTO
// =======================================================================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao aplicar migrations: {ex.Message}");
    }
}


// =======================================================================
// MIDDLEWARES
// =======================================================================
app.UseMiddleware<TratamentoDeErros>();
app.UseSwagger();
app.UseSwaggerUI();


var envApp = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
if (envApp == "Development")
    app.UseCors("dev");
else
    app.UseCors("prd");

// 🔥 Fix global do preflight (sem exigir auth)
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next.Invoke();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"🌍 Ambiente ativo: {envApp}");
Console.ResetColor();
app.Run();
