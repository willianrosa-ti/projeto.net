namespace PrimeiraApi.Models
{
    // DTO (Data Transfer Object) para informar o frontend sobre o status da conexão
    public class StatusConexaoDto
    {
        // Status pode ser: "Nenhum", "Pendente", "PendenteInverso", "Aceito"
        public required string Status { get; set; } 
        
        // Se houver uma conexão, enviamos o ID (Identificador) dela (útil para Aceitar/Recusar)
        public int? ConexaoId { get; set; }
    }
}