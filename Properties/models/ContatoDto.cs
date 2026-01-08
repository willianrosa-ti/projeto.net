namespace PrimeiraApi.Models
{
    // Um DTO (Data Transfer Object - Objeto de Transferência de Dados) simples para listar os contatos
    public class ContatoDto
    {
        public int Id { get; set; } // Id do Usuário (não da conexão)
        public string? Nome { get; set; }
        public string? FotoUrl { get; set; }
    }
}
