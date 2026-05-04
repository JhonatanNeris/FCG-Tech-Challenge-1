using FCG.API.Endpoints;
using FCG.API.Middlewares;
using FCG.Application.Interfaces;
using FCG.Application.Services;
using FCG.Application.Settings;
using FCG.Application.Validators;
using FCG.Domain.Interfaces;
using FCG.Infrastructure.Data;
using FCG.Infrastructure.Repositories;
using FCG.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A configuracao 'ConnectionStrings:DefaultConnection' e obrigatoria.");
var secretKey = builder.Configuration["chave_secreta"]
    ?? throw new InvalidOperationException("A configuracao 'chave_secreta' e obrigatoria.");
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("A configuracao 'Jwt:Key' e obrigatoria.");
var pageSize = builder.Configuration.GetValue<int?>("Pagination:PageSize") ?? 30;
if (pageSize < 1)
{
    throw new InvalidOperationException("A configuracao 'Pagination:PageSize' deve ser maior ou igual a 1.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, x => x.MigrationsAssembly("FCG.Infrastructure"))
        .LogTo(Console.WriteLine, LogLevel.Information),
    ServiceLifetime.Scoped);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton(new AuthSettings(secretKey));
builder.Services.AddSingleton(new JwtSettings(jwtKey));
builder.Services.AddSingleton(new PaginationSettings(pageSize));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no campo abaixo comecando com 'Bearer '. Exemplo: Bearer eyJhb..."
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        await DatabaseSeeder.SeedAsync(context, secretKey);
        Console.WriteLine("Banco de dados atualizado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ocorreu um erro ao aplicar as migracoes: {ex.Message}");
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<UserLoggingScopeMiddleware>();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapGameEndpoints();
app.MapPromotionEndpoints();
app.MapOrderEndpoints();

app.Run();
