using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

try
{


    builder.Services.AddSwaggerGen();
}
catch (Exception e)
{
    Console.WriteLine("Error: " + e.Message);
}

builder.Services.AddDbContext<API_NEXORA_INGSW.Models.DbContextNexora>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("StringConexion")));

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
