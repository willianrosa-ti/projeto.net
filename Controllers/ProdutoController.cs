using Microsoft.AspNetCore.Mvc;

namespace PrimeiraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var produtos = new[] { "Carro", "Moto", "Bicicleta", "Camimnhão", "Barco", "Lancha" };
            return Ok(produtos);
        }
    }
}
