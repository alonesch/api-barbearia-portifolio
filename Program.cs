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
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
using System.Net;
using Microsoft.AspNetCore.RateLimiting;

DotNetEnv.Env.Load();
// =======================================================================
// BUILDER
// =======================================================================
var builder = WebApplication.CreateBuilder(args);


// =======================================================================
// CONTROLLERS + SWAGGER
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
        Description = "Cole aqui o token JWT"
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

// =======================================================================
// JWT
// =======================================================================
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
        builder.Configuration["Jwt:Key"]
        ?? Environment.GetEnvironmentVariable("JWT_KEY")
        ?? throw new Exception("JWT_KEY não configurada")
             )
        ),

            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// =======================================================================
// AUTHORIZATION (ROLES)
// =======================================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOuBarbeiro", policy =>
        policy.RequireRole("Admin", "Barbeiro")
    );
});

// =======================================================================
// RATE LIMITING (LOGIN)
// =======================================================================
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            })
    );

    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsync(
            """{ "erro": "Muitas tentativas. Aguarde 1 minuto para tentar novamente." }"""
        );
    };
    options.AddPolicy("ConfirmarEmailPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress;

        
        var partitionKey = ip?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });


});

// =======================================================================
// DATABASE  
// =======================================================================
var env = builder.Environment.EnvironmentName;

Console.WriteLine($"[BOOT] ASPNETCORE_ENVIRONMENT = {env}");
Console.WriteLine($"[BOOT] POSTGRES_CONNECTION_DEV = [{Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_DEV")}]");
Console.WriteLine($"[BOOT] POSTGRES_CONNECTION = [{Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")}]");

var pgConnection =
    Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
    ?? (env == Environments.Development
        ? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_DEV")
        : null);


if (string.IsNullOrWhiteSpace(pgConnection))
{
    throw new Exception($"String de conexão Postgres não encontrada para ambiente: {env}");
}

builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(pgConnection));

// =======================================================================
// CORS
// =======================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("prd", p =>
        p.WithOrigins("https://www.barbercloud.online")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());

    options.AddPolicy("dev", p =>
        p.WithOrigins(
            "https://dev.barbercloud.online",
            "http://localhost:5173",
            "http://localhost:3000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
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
// KESTREL
// =======================================================================
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

builder.WebHost.UseUrls("http://0.0.0.0:8080");

// =======================================================================
// BUILD
// =======================================================================
var app = builder.Build();

// =======================================================================
// GLOBAL ERROR HANDLER
// =======================================================================
app.UseWhen(
    context => context.Request.Method != HttpMethods.Options,
    appBuilder => appBuilder.UseMiddleware<TratamentoDeErros>()
);


// =======================================================================
// MIGRATIONS ON STARTUP
// =======================================================================
//if (!builder.Environment.IsEnvironment("DesignTime"))
//{
//    using var scope = app.Services.CreateScope();
//    try
//    {
//        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
//        db.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"[MIGRATION SKIPPED] {ex.Message}");
//    }
//}



// =======================================================================
// SWAGGER (DEV)
// =======================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =======================================================================
// PIPELINE
// =======================================================================
app.UseRouting();

app.UseCors(env == Environments.Development ? "dev" : "prd");

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseWhen(
    context => context.Request.Method != HttpMethods.Options,
    appBuilder => appBuilder.UseRateLimiter()
    );

app.UseAuthentication();
app.UseAuthorization();

// HEALTHCHECK
app.MapGet("/ping", () => Results.Ok("pong"));

// CONTROLLERS
app.MapControllers();

Console.WriteLine($"Ambiente ativo: {env}");



// =======================================================================
// RUN
// =======================================================================
app.Run();
