// Em 'PrimeiraApi.Models/FotoUsuarioDto.cs'

using System.ComponentModel.DataAnnotations;

namespace PrimeiraApi.Models
{
    public class FotoUsuarioDto
    {
        [Required]
        public int UsuarioId { get; set; } // O ID (Identificador) do usuário a ser atualizado

        // 🟢 NOVO: O ID (Identificador) de quem está fazendo a requisição
        [Required]
        public int UsuarioLogadoId { get; set; } 

        [Required]
        [StringLength(500)]
        public required string FotoUrl { get; set; }
    }
}