using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

/*//база данных будет храниться только в памяти. Обычно используется для тестирования.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDatabase"));*/

// Добавляем поддержку контроллеров
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



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

