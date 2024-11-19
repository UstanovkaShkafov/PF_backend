using Microsoft.EntityFrameworkCore;
using Putov_backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контекст базы данных
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDatabase"));

// Добавляем поддержку контроллеров
builder.Services.AddControllers();

var app = builder.Build();

// Настройка маршрутов для API-контроллеров
app.MapControllers();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.Run();

