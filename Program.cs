using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebProtectorApi.Data;
using WebProtectorApi.Services;
using WebProtectorApi.Extensions;
using WebProtectorApi.Entities;

var builder = WebApplication.CreateBuilder(args);

// 1. Database & Controllers
builder.Services.AddControllers();

// База данных
builder.Services.AddDbContext<WebProtectorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. CORS (Add this block to fix the "Server error")
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Services
// AddHttpClient already registers IScannerService as Scoped, 
// so you don't need builder.Services.AddScoped again (Clean Code tip!)
builder.Services.AddHttpClient<IScannerService, ScannerService>();

// 4. Auth
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

// 5. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// --- 2. BUILD (Сборка приложения - вызывается ОДИН раз) ---
var app = builder.Build();

// 6. Pipeline (THE ORDER MATTERS HERE!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CRITICAL: UseCors MUST be before Authentication and Authorization
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

// CORS всегда ПЕРЕД Auth
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run(); ;
