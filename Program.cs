using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebProtectorApi.Data;
using WebProtectorApi.Services;
using WebProtectorApi.Extensions;
using WebProtectorApi.Entities;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICES (Настройка сервисов) ---

builder.Services.AddControllers();

// База данных
builder.Services.AddDbContext<WebProtectorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS - разрешаем всё для фронтенда
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Сервисы сканера
builder.Services.AddHttpClient<IScannerService, ScannerService>();
builder.Services.AddScoped<IScannerService, ScannerService>();

// Настройка JWT Auth
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "DefaultSuperSecretKey1234567890");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// --- 2. BUILD (Сборка приложения - вызывается ОДИН раз) ---
var app = builder.Build();

// --- 3. PIPELINE (Настройка промежуточного ПО) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS всегда ПЕРЕД Auth
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();