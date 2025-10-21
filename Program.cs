using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Context;

var builder = WebApplication.CreateBuilder(args);

// Adicionar o Serviço CORS (NOVO)
builder.Services.AddCors(options =>
{
    // Define uma política chamada "AllowAll" que é segura para ambientes de desenvolvimento local.
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin() // Permite qualquer origem (seu HTML local)
                          .AllowAnyHeader()   // Permite qualquer tipo de cabeçalho
                          .AllowAnyMethod());  // Permite qualquer método (POST, GET, etc.)
});
// ------------------------------------

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure o DbContext para usar o SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use a política CORS (NOVO)
app.UseCors("AllowAll"); 
// ------------------------------------

app.UseAuthorization();

app.MapControllers();

app.Run();