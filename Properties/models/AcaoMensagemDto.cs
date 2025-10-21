namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) para o frontend aceitar/recusar (Req 4 e 5)
    public class AcaoMensagemDto
    {
        public int SolicitacaoId { get; set; }
        public int UsuarioLogadoId { get; set; }
    }
}