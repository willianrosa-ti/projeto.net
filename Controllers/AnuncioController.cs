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

        // =================================================================================
        // 🟢 Endpoint de listagem (GET: api/Anuncio/listar)
        // =================================================================================
        [HttpGet("listar")]
        // Troca o retorno de Anuncio para AnuncioDto
        public async Task<ActionResult<IEnumerable<AnuncioDto>>> Listar()
        {
            // Busca todos os Anúncios, incluindo o Usuário
            var anuncios = await _context.Anuncios
                                         .Include(a => a.Usuario)
                                         .OrderByDescending(a => a.DataCriacao) // Mais novo primeiro
                                         .ToListAsync();

            if (anuncios == null || !anuncios.Any())
            {
                return Ok(new List<AnuncioDto>());
            }

            // Mapeia (converte) Anuncio (Entity) para AnuncioDto
            var anuncioDtos = anuncios.Select(a => new AnuncioDto
            {
                Id = a.Id,
                UsuarioId = a.UsuarioId,
                Titulo = a.Titulo,
                Descricao = a.Descricao,
                Preco = a.Preco,
                DataCriacao = a.DataCriacao,
                // Assumindo que seu modelo Usuario tem uma propriedade NomeCompleto ou Nome
                UsuarioNome = a.Usuario?.Nome ?? "Usuário Desconhecido",
                UsuarioFotoUrl = a.Usuario?.FotoUrl
                
                
            }).ToList();

            return Ok(anuncioDtos);
        }
        
        // =================================================================================
        // 🟢 NOVO: Endpoint para listar anúncios por ID (Identificador) do Usuário
        // GET: api/Anuncio/listarPorUsuario/5
        // =================================================================================
        [HttpGet("listarPorUsuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<AnuncioDto>>> ListarPorUsuario(int usuarioId)
        {
            // Busca todos os Anúncios do usuário, ordenando do mais novo para o mais antigo
            var anuncios = await _context.Anuncios
                                         .Include(a => a.Usuario)
                                         .Where(a => a.UsuarioId == usuarioId) // FILTRA pelo ID (Identificador) do Usuário
                                         .OrderByDescending(a => a.DataCriacao) 
                                         .ToListAsync();

            if (anuncios == null || !anuncios.Any())
            {
                return Ok(new List<AnuncioDto>()); // Retorna uma lista vazia se não houver posts
            }

            // Mapeia para AnuncioDto para incluir nome e foto
            var anunciosDto = anuncios.Select(a => new AnuncioDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                Descricao = a.Descricao,
                Preco = a.Preco,
                UsuarioId = a.UsuarioId,
                DataCriacao = a.DataCriacao,
                UsuarioNome = a.Usuario?.Nome,
                UsuarioFotoUrl = a.Usuario?.FotoUrl
            }).ToList();

            return Ok(anunciosDto);
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