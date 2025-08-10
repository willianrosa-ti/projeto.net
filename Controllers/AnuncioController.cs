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

            // GET: api/Anuncio
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Anuncio>>> GetAnuncios()
            {
                var anuncios = await _context.Anuncios
                                             .Include(a => a.Usuario)
                                             .ToListAsync();
                
                return Ok(anuncios);
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

                anuncio.Usuario = usuario;

                return CreatedAtAction(nameof(GetAnuncio), new { id = anuncio.Id }, anuncio);
            }
            
            // PUT: api/Anuncio/editar/5
            [HttpPut("editar/{id}")]
            public async Task<IActionResult> PutAnuncio(int id, AnuncioDto anuncioDto)
            {
                var anuncio = await _context.Anuncios.FindAsync(id);
                if (anuncio == null)
                {
                    return NotFound(new { message = "Anúncio não encontrado." });
                }

                // Atualiza as propriedades do anúncio existente com os dados do DTO
                anuncio.Titulo = anuncioDto.Titulo;
                anuncio.Descricao = anuncioDto.Descricao;
                anuncio.Preco = anuncioDto.Preco;
                
                await _context.SaveChangesAsync();
                
                return NoContent();
            }

            // DELETE: api/Anuncio/deletar/5
            [HttpDelete("deletar/{id}")]
            public async Task<IActionResult> DeleteAnuncio(int id)
            {
                var anuncio = await _context.Anuncios.FindAsync(id);
                if (anuncio == null)
                {
                    return NotFound(new { message = "Anúncio não encontrado." });
                }

                _context.Anuncios.Remove(anuncio);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    }
    