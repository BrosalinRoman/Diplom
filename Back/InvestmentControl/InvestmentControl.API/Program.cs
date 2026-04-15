using FluentValidation;
using InvestmentControl.API.DTOs.Responses;
using InvestmentControl.API.Middleware;
using InvestmentControl.Application.Analytics.Queries;
using InvestmentControl.Application.Common.Behaviors;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Infrastructure.Data;
using InvestmentControl.Infrastructure.Repositories;
using InvestmentControl.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .SelectMany(e => e.Value.Errors.Select(x => x.ErrorMessage))
                .ToList();
            return new BadRequestObjectResult(new ErrorResponse { Error = string.Join("; ", errors) });
        };
    });


builder.Services.AddEndpointsApiExplorer();

// ===== НАСТРОЙКА SWAGGER С JWT =====
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InvestmentControl API",
        Version = "v1",
        Description = "API для управления инвестиционными проектами"
    });

    // Определяем схему безопасности Bearer
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Введите JWT токен в формате: Bearer <token>",
        Name = "Authorization",
        In = ParameterLocation.Header,
    });

    // Требуем использовать эту схему для всех эндпоинтов с [Authorize]
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

// Настройка JWT аутентификации
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"] ?? throw new Exception("JWT Secret not configured");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddAuthorization();

// Добавляем DbContext (для PostgreSQL)
builder.Services.AddDbContext<ReadOnlyAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<ControlDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Регистрируем репозитории
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IInvestmentRepository, InvestmentRepository>();
builder.Services.AddScoped<ICostRepository, CostRepository>();
builder.Services.AddScoped<IProgressReportRepository, ProgressReportRepository>();
builder.Services.AddScoped<IProjectReadRepository, ProjectReadRepository>();

// Регистрируем сервис текущего пользователя
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();

// Регистрируем MediatR (сканируем сборку Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProjectsAnalyticsQuery).Assembly));
// В Program.cs после AddMediatR
builder.Services.AddValidatorsFromAssembly(typeof(GetProjectsAnalyticsQuery).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Регистрируем AutoMapper (если используется)
// builder.Services.AddAutoMapper(typeof(Program).Assembly); // можно добавить позже

var app = builder.Build();

// Сначала middleware обработки исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Настройка конвейера
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
