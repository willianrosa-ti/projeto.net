using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PrimeiraApi.Context;
// Esta classe é uma "fábrica" que o 'dotnet ef' usa para criar o DbContext
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // IMPORTANTE: Insira sua Connection String para o LocalDB aqui
        optionsBuilder.UseSqlServer("Server=.;Database=PrimeiraApi;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}