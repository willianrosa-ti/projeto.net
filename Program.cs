var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a controllers (API REST)
builder.Services.AddControllers();

// Adiciona suporte ao Swagger (documentação da API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ativa o Swagger SEM o "if", sempre aparece
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
