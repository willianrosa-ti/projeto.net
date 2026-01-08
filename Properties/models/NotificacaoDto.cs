using System; // Necessário para o DateTime

namespace PrimeiraApi.Models
{
    public class NotificacaoDto
    {
        public int ConexaoId { get; set; }
        public int SolicitanteId { get; set; }
        public string? SolicitanteNome { get; set; }
        public string? SolicitanteFotoUrl { get; set; }
        public DateTime DataSolicitacao { get; set; }
    }
}
