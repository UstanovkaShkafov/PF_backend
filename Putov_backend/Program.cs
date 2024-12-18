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

// ��������� ���� ������
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� ��������� ������������ � ���������� ReferenceHandler.Preserve
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ��������� �������������� � �����������
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

// ��������� �������� �����������
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin")); // ���������� lowercase
    options.AddPolicy("user", policy => policy.RequireRole("user")); // ���������� lowercase
});


var app = builder.Build();


// ��������� ��������� ��� API-������������
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

// ���������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

builder.Logging.AddConsole();
IdentityModelEventSource.ShowPII = true; // ���������� ������ ������ �������



app.Run();
