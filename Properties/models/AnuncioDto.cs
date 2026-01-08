using System.ComponentModel.DataAnnotations;

namespace PrimeiraApi.Models
{
    public class AnuncioDto
    {
        // NOVO: Adicione o Id para ser exibido no feed (e para futura edição/exclusão)
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Titulo { get; set; }

        [StringLength(10000)]
        public string? Descricao { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        // NOVO: Adicione a data de criação para ser exibida no feed
        public DateTime DataCriacao { get; set; }

        // NOVO: Adicione o nome do usuário para ser exibido no feed
        public string? UsuarioNome { get; set; } 
        
         public string? UsuarioFotoUrl { get; set; } 
    }
}