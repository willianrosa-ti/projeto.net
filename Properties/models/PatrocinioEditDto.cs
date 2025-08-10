using System;
using System.ComponentModel.DataAnnotations;

namespace PrimeiraApi.Models
{
    public class PatrocinioEditDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Titulo { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [Required]
        public int UsuarioId { get; set; }
    }
}
