using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sistema_RRHH_Nexora.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Sistema_RRHH_NexoraContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sistema_RRHH_NexoraContext") ?? throw new InvalidOperationException("Connection string 'Sistema_RRHH_NexoraContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
