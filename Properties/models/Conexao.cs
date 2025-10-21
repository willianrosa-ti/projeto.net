using System; // 🟢 CORREÇÃO: Adicionando o namespace que faltava
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeiraApi.Models
{
    // Enum para definir os status possíveis da conexão
    public enum StatusConexao
    {
        Pendente, // 0 - O usuário solicitou, mas o outro não aceitou
        Aceito,   // 1 - Conexão mútua
        Recusado  // 2 - O usuário recusou a solicitação
    }

    [Table("Conexoes")]
    public class Conexao
    {
        [Key]
        public int Id { get; set; }

        // Quem enviou o pedido (O Seguidor)
        [Required]
        public int SolicitanteId { get; set; }
        [ForeignKey("SolicitanteId")]
        public Usuario? Solicitante { get; set; } // Navegação

        // Quem recebeu o pedido (O Seguido)
        [Required]
        public int SolicitadoId { get; set; }
        [ForeignKey("SolicitadoId")]
        public Usuario? Solicitado { get; set; } // Navegação

        [Required]
        public StatusConexao Status { get; set; }

        public DateTime DataSolicitacao { get; set; } = DateTime.Now;
        
        // Data que foi aceito ou recusado
        public DateTime? DataConfirmacao { get; set; } 
    }
}