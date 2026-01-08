using System;

namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) para o NotificationsWidget (Req 3 - Widget)
    public class NotificacaoMensagemDto
    {
        public int Id { get; set; } // ID (Identificador) da Solicitação
        public int SolicitanteId { get; set; }
        public string? SolicitanteNome { get; set; }
        public string? SolicitanteFotoUrl { get; set; }
        public DateTime DataSolicitacao { get; set; }
    }
}
