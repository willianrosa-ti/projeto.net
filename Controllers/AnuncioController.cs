using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Context;
using PrimeiraApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeiraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnuncioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnuncioController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Anuncio/cadastrar
        [HttpPost("cadastrar")]
        public async Task<ActionResult<Anuncio>> PostAnuncio(Anuncio anuncio)
        {
            // Validação de que o usuário existe antes de cadastrar o anúncio
            var usuario = await _context.Usuarios.FindAsync(anuncio.UsuarioId);
            if (usuario == null)
            {
                return BadRequest(new { message = "O usuário associado ao anúncio não existe." });
            }

            _context.Anuncios.Add(anuncio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnuncio), new { id = anuncio.Id }, anuncio);
        }

        // GET: api/Anuncio/buscar/5
        [HttpGet("buscar/{id}")]
        public async Task<ActionResult<Anuncio>> GetAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);

            if (anuncio == null)
            {
                return NotFound();
            }

            return anuncio;
        }
    }
}
