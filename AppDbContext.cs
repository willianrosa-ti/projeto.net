using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Models; // <<-- Importa a sua classe Produto

namespace PrimeiraApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Anuncio> Anuncios { get; set; }
       
       public DbSet<Patrocinio> Patrocinios { get; set; }
    }
}

