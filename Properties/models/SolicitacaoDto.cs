namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) simples para receber o pedido de solicitação
    public class SolicitacaoDto
    {
        public int SolicitanteId { get; set; } // ID (Identificador) de quem clica
        public int SolicitadoId { get; set; }  // ID (Identificador) do dono do perfil
    }
}