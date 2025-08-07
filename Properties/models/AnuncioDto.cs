    using System.ComponentModel.DataAnnotations;

    namespace PrimeiraApi.Models
    {
        public class AnuncioDto
        {
            [Required]
            [StringLength(100)]
            public required string Titulo { get; set; }

            [StringLength(500)]
            public string? Descricao { get; set; }

            [Required]
            public decimal Preco { get; set; }

            [Required]
            public int UsuarioId { get; set; }
        }
    }
    