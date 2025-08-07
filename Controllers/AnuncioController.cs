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
            public async Task<ActionResult<Anuncio>> PostAnuncio(AnuncioDto anuncioDto)
            {
                var usuario = await _context.Usuarios.FindAsync(anuncioDto.UsuarioId);
                if (usuario == null)
                {
                    return BadRequest(new { message = "O usuário associado ao anúncio não existe." });
                }

                var anuncio = new Anuncio
                {
                    Titulo = anuncioDto.Titulo,
                    Descricao = anuncioDto.Descricao,
                    Preco = anuncioDto.Preco,
                    UsuarioId = anuncioDto.UsuarioId
                };

                _context.Anuncios.Add(anuncio);
                await _context.SaveChangesAsync();

                // Inclui a informação do usuário na resposta para que o Swagger retorne o objeto completo
                anuncio.Usuario = usuario;

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
    