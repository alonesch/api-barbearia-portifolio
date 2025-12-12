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
using System.Threading.RateLimiting;




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

// =======================================================================
// JWT CONFIG
// =======================================================================
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireClaim("cargo", "Admin"));
    options.AddPolicy("Barbeiro", p => p.RequireClaim("cargo", "Barbeiro"));
    options.AddPolicy("Cliente", p => p.RequireClaim("cargo", "Cliente"));

    options.AddPolicy("AdminOuBarbeiro", policy =>
        policy.RequireAssertion(ctx =>
        {
            var cargo = ctx.User.FindFirst("cargo")?.Value;
            return cargo == "Admin" || cargo == "Barbeiro";
        })
    );
});

// =======================================================================
// RATE LIMITING
// =======================================================================
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: key => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, // 5 tentativas por minuto por IP
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            })
    );

    options.OnRejected = async (context, _) =>
    {
        Console.WriteLine($"[RateLimit] IP bloqueado: {context.HttpContext.Connection.RemoteIpAddress}");

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var json = """{ "erro": "Muitas tentativas. Aguarde 1 minuto para tentar novamente." }""";

        await context.HttpContext.Response.WriteAsync(json);
    };

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
// JWT AUTHENTICATION
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
    options.AddPolicy("prd", p =>
        p.WithOrigins("https://barbearia-gabriel-port.vercel.app")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());

    options.AddPolicy("dev", p =>
        p.WithOrigins(
            "https://dev-barbearia-gabriel-port.vercel.app",
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
// DEBUG REQUEST LOGGING (DEV ONLY)
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
            Console.WriteLine(" EXCEÇÃO NÃO TRATADA:");
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
// GLOBAL ERROR HANDLER
// =======================================================================
app.UseMiddleware<TratamentoDeErros>();

// =======================================================================
// APPLY MIGRATIONS ON BOOT
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
        Console.WriteLine($" Erro ao aplicar migrations: {ex.Message}");
    }
}

// =======================================================================
// SWAGGER (DEV ONLY)
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

// Ignorar OPTIONS
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

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

    await next();
});

// =======================================================================
// RATE LIMITER MIDDLEWARE
// =======================================================================
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// HEALTHCHECK
app.MapGet("/ping", () => Results.Ok("pong"));

// CONTROLLERS
app.MapControllers();

Console.WriteLine($"Ambiente ativo: {envApp}");

// =======================================================================
// RUN
// =======================================================================
app.Run();
