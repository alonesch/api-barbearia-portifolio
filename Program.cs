using BarbeariaPortifolio.API.Auth;
using BarbeariaPortifolio.API.Data;
using BarbeariaPortifolio.API.Middleware;
using BarbeariaPortifolio.API.Models;
using BarbeariaPortifolio.API.Repositorios;
using BarbeariaPortifolio.API.Repositorios.Interfaces;
using BarbeariaPortifolio.API.Servicos;
using BarbeariaPortifolio.API.Servicos.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

// =======================================================================
// BUILDER
// =======================================================================
var builder = WebApplication.CreateBuilder(args);

// =======================================================================
// SWAGGER
// =======================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Barbearia API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole aqui o token {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    
    c.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
});



builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireClaim("cargo", "Admin"));

    options.AddPolicy("Barbeiro", policy =>
        policy.RequireClaim("cargo", "Barbeiro"));

    options.AddPolicy("Cliente", policy =>
        policy.RequireClaim("cargo", "Cliente"));

    options.AddPolicy("AdminOuBarbeiro", policy =>
        policy.RequireAssertion(ctx =>
        {
            var cargo = ctx.User.FindFirst("cargo")?.Value;
            return cargo == "Admin" || cargo == "Barbeiro";
        }));
});


// =======================================================================
// DATABASE
// =======================================================================
var envApp = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var pgConnection = builder.Configuration.GetConnectionString("Postgres");

if (string.IsNullOrWhiteSpace(pgConnection))
{
    pgConnection = Environment.GetEnvironmentVariable(
        envApp == "Development" ? "POSTGRES_CONNECTION_DEV" : "POSTGRES_CONNECTION");
}

if (string.IsNullOrWhiteSpace(pgConnection))
{
    throw new Exception("String de conexão 'Postgres' não encontrada!");
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(pgConnection));

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
            RequireSignedTokens = true,
            RequireExpirationTime = true,
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
    options.AddPolicy("prd", policy =>
        policy.WithOrigins("https://barbearia-gabriel-port.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
    );

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
builder.Services.AddScoped<IDisponibilidadeServico, DisponibilidadeServico>();
builder.Services.AddScoped<IEmailConfirmacaoTokenRepositorio, EmailConfirmacaoTokenRepositorio>();
builder.Services.AddScoped<IEmailServico, EmailServico>();

// =======================================================================
// KESTREL OPTIMIZATION
// =======================================================================
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

ThreadPool.SetMinThreads(100, 100);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// =======================================================================
// BUILD APP
// =======================================================================
var app = builder.Build();

// =======================================================================
// ERROR HANDLING DEBUG (DEV only)
// =======================================================================
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        try
        {
            Console.WriteLine($"🔵 REQUEST: {context.Request.Method} {context.Request.Path}");
            await next();
            Console.WriteLine($"✅ RESPONSE: {context.Response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("🔥 EXCEÇÃO NÃO TRATADA:");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Type: {ex.GetType().Name}");
            Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            Console.WriteLine("==========================================");
            throw;
        }
    });
}

// =======================================================================
// ERROR HANDLING
// =======================================================================
app.UseMiddleware<TratamentoDeErros>();

// =======================================================================
// ERROR HANDLING
// =======================================================================
app.UseMiddleware<TratamentoDeErros>();

// =======================================================================
// MIGRATIONS ON BOOT
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
// SWAGGER (DEV only)
// =======================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =======================================================================
// MIDDLEWARE PIPELINE
// =======================================================================
app.UseRouting();

app.UseCors(envApp == "Development" ? "dev" : "prd");

app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// HEALTHCHECK
app.MapGet("/ping", () => Results.Ok("pong"));

// CONTROLLERS
app.MapControllers();

Console.WriteLine($"🌍 Ambiente ativo: {envApp}");

// =======================================================================
// RUN APP
// =======================================================================
app.Run();
