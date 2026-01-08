using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Models; // Necessário para Usuario, Anuncio, Conexao e Patrocinio

namespace PrimeiraApi.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Nossas tabelas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<Conexao> Conexoes { get; set; }
        public DbSet<Patrocinio> Patrocinios { get; set; }

        // 🟢 NOVAS TABELAS DE MENSAGEM
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<SolicitacaoMensagem> SolicitacoesMensagem { get; set; }


        // Método único para configurar o modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para Conexoes (Existente)
            modelBuilder.Entity<Conexao>()
                .HasOne(c => c.Solicitante)
                .WithMany() 
                .HasForeignKey(c => c.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict); // Impede deleção em cascata

            modelBuilder.Entity<Conexao>()
                .HasOne(c => c.Solicitado)
                .WithMany()
                .HasForeignKey(c => c.SolicitadoId)
                .OnDelete(DeleteBehavior.Restrict); // Impede deleção em cascata
            
            // 🟢 NOVAS CONFIGURAÇÕES DE MENSAGEM

            // Configuração para SolicitacaoMensagem
            modelBuilder.Entity<SolicitacaoMensagem>()
                .HasOne(s => s.Solicitante)
                .WithMany()
                .HasForeignKey(s => s.SolicitanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitacaoMensagem>()
                .HasOne(s => s.Solicitado)
                .WithMany()
                .HasForeignKey(s => s.SolicitadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração para Mensagem
            modelBuilder.Entity<Mensagem>()
                .HasOne(m => m.Remetente)
                .WithMany()
                .HasForeignKey(m => m.RemetenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mensagem>()
                .HasOne(m => m.Destinatario)
                .WithMany()
                .HasForeignKey(m => m.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}