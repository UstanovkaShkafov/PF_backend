using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Putov_backend.Data;
using Putov_backend;
using Putov_backend.Models;
using Microsoft.IdentityModel.Logging;


var builder = WebApplication.CreateBuilder(args);

// Настройка базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем поддержку контроллеров с настройкой ReferenceHandler.Preserve
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Добавляем аутентификацию и авторизацию
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = AuthOptions.Issuer,
            ValidAudience = AuthOptions.Audience,
            IssuerSigningKey = AuthOptions.SigningKey
        };
    });

// Добавляем политики авторизации
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin")); // Используем lowercase
    options.AddPolicy("user", policy => policy.RequireRole("user")); // Используем lowercase
});


var app = builder.Build();


// Настройка маршрутов для API-контроллеров
app.MapControllers();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Подключаем аутентификацию и авторизацию
app.UseAuthentication();
app.UseAuthorization();

builder.Logging.AddConsole();
IdentityModelEventSource.ShowPII = true; // Показывает детали ошибок токенов



app.Run();
