using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Data;
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Barbearia Portifolio", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Digite: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();
builder.Services.AddAuthorization();

// =======================================================================
// DATABASE
// =======================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("⚠️ Nenhuma connection string encontrada. Usando fallback...");
    Console.ResetColor();

    connectionString =
        "Server=yamabiko.proxy.rlwy.net;Port=15819;Database=railway;User=root;Password=FwIAsbobfoGSFUrfLCSLNrtauWZtPTZN;SslMode=Preferred;";
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


// =======================================================================
// JWT AUTH
// =======================================================================
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
var keyValue = builder.Configuration["Jwt:Key"] ?? string.Empty;

if (string.IsNullOrWhiteSpace(keyValue))
{
    Console.ForegroundColor = ConsoleColor.Red;
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
// CORS
// =======================================================================
builder.Services.AddCors(options =>
{
    // DEV
    options.AddPolicy("dev", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://barbearia-gabriel-port-git-dev-alonesch.vercel.app",
                "https://dev-barbearia-gabriel-port.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );

    // PRD
    options.AddPolicy("prd", policy =>
        policy.WithOrigins("https://barbearia-gabriel-port.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
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
// KESTREL (RAILWAY)
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
// AUTO MIGRATE
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
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsProduction())
    app.UseCors("prd");
else
    app.UseCors("dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("API Online ✔");
Console.ResetColor();

app.Run();
