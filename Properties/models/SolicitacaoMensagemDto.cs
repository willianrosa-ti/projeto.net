namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) para o frontend enviar um pedido (Req 3)
    public class SolicitacaoMensagemDto
    {
        public int SolicitanteId { get; set; }
        public int SolicitadoId { get; set; }
    }
}