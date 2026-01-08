using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeiraApi.Models
{
    // Enum para o status da solicitação
    public enum StatusSolicitacao
    {
        Pendente, // 0
        Aceito,   // 1
        Recusado  // 2
    }

    [Table("SolicitacoesMensagem")]
    public class SolicitacaoMensagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SolicitanteId { get; set; }
        [ForeignKey("SolicitanteId")]
        public Usuario? Solicitante { get; set; }

        [Required]
        public int SolicitadoId { get; set; }
        [ForeignKey("SolicitadoId")]
        public Usuario? Solicitado { get; set; }

        [Required]
        public StatusSolicitacao Status { get; set; }

        public DateTime DataSolicitacao { get; set; } = DateTime.Now;
    }
}