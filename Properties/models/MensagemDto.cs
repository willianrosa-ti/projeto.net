using System;

namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) para o histórico (Req 2) e para enviar novas mensagens
    public class MensagemDto
    {
        public int Id { get; set; }
        public int RemetenteId { get; set; }
        public int DestinatarioId { get; set; }
        public required string Conteudo { get; set; }
        public DateTime DataEnvio { get; set; }
    }
}