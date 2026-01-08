using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeiraApi.Models
{
    [Table("Mensagens")]
    public class Mensagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RemetenteId { get; set; }
        [ForeignKey("RemetenteId")]
        public Usuario? Remetente { get; set; }

        [Required]
        public int DestinatarioId { get; set; }
        [ForeignKey("DestinatarioId")]
        public Usuario? Destinatario { get; set; }

        [Required]
        [StringLength(2000)]
        public required string Conteudo { get; set; }

        public DateTime DataEnvio { get; set; } = DateTime.Now;

        public bool Lido { get; set; } = false;
    }
}