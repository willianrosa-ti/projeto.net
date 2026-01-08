namespace PrimeiraApi.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public required string Nome { get; set; } // <-- Adicione 'required' aqui
    }
}
