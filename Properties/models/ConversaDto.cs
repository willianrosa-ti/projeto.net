using System;

namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) para a lista de conversas (Req 1)
    public class ConversaDto
    {
        public int OutroUsuarioId { get; set; }
        public string? OutroUsuarioNome { get; set; }
        public string? OutroUsuarioFotoUrl { get; set; }
        public string? UltimaMensagem { get; set; }
        public DateTime DataUltimaMensagem { get; set; }
    }
}