namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object - Objeto de Transferência de Dados) para receber a ação de aceitar/recusar
    public class AcaoConexaoDto
    {
        // O ID (Identificador) da conexão (que está pendente)
        public int ConexaoId { get; set; }
        
        // O ID (Identificador) do usuário logado (para garantir que ele é o 'Solicitado')
        public int UsuarioLogadoId { get; set; } 
    }
}