using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeiraApi.Models
{
    public class Anuncio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Titulo { get; set; }

        [StringLength(10000)]
        public string? Descricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

               

         
    }
}
