using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeiraApi.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Login { get; set; } // Adicionei 'required'

        [Required]
        [StringLength(100)]
        public required string Senha { get; set; } // Adicionei 'required'


        [StringLength(100)]
        public required string Nome { get; set; }

        [StringLength(150)]
        public required string Email { get; set; }

        [StringLength(14)] // CNPJ possui 14 dígitos
        public required string CNPJ { get; set; }

        [StringLength(15)] // Para o número de celular
        public required string Celular { get; set; }
    }
}