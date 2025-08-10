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
    public class PatrocinioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatrocinioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Patrocinio - Retorna todos os patrocínios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patrocinio>>> GetPatrocinios()
        {
            var patrocinios = await _context.Patrocinios
                                            .Include(p => p.Usuario)
                                            .ToListAsync();
            
            return Ok(patrocinios);
        }
        
        // POST: api/Patrocinio/cadastrar
        [HttpPost("cadastrar")]
        public async Task<ActionResult<Patrocinio>> PostPatrocinio(PatrocinioDto patrocinioDto)
        {
            var usuario = await _context.Usuarios.FindAsync(patrocinioDto.UsuarioId);
            if (usuario == null)
            {
                return BadRequest(new { message = "O usuário associado ao patrocínio não existe." });
            }

            var patrocinio = new Patrocinio
            {
                Titulo = patrocinioDto.Titulo,
                Descricao = patrocinioDto.Descricao,
                Valor = patrocinioDto.Valor,
                DataInicio = patrocinioDto.DataInicio,
                DataFim = patrocinioDto.DataFim,
                UsuarioId = patrocinioDto.UsuarioId
            };

            _context.Patrocinios.Add(patrocinio);
            await _context.SaveChangesAsync();
            
            patrocinio.Usuario = usuario;

            return CreatedAtAction(nameof(GetPatrocinios), new { id = patrocinio.Id }, patrocinio);
        }
        
        // PUT: api/Patrocinio/editar/5
        [HttpPut("editar/{id}")]
        public async Task<IActionResult> PutPatrocinio(int id, PatrocinioEditDto patrocinioEditDto)
        {
            if (id != patrocinioEditDto.Id)
            {
                return BadRequest(new { message = "O ID do patrocínio na URL não corresponde ao ID no corpo da requisição." });
            }
            
            var patrocinio = await _context.Patrocinios.FindAsync(id);
            if (patrocinio == null)
            {
                return NotFound(new { message = "Patrocínio não encontrado." });
            }
            
            patrocinio.Titulo = patrocinioEditDto.Titulo;
            patrocinio.Descricao = patrocinioEditDto.Descricao;
            patrocinio.Valor = patrocinioEditDto.Valor;
            patrocinio.DataInicio = patrocinioEditDto.DataInicio;
            patrocinio.DataFim = patrocinioEditDto.DataFim;
            
            _context.Entry(patrocinio).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatrocinioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return NoContent();
        }

        // DELETE: api/Patrocinio/deletar/5
        [HttpDelete("deletar/{id}")]
        public async Task<IActionResult> DeletePatrocinio(int id)
        {
            var patrocinio = await _context.Patrocinios.FindAsync(id);
            if (patrocinio == null)
            {
                return NotFound(new { message = "Patrocínio não encontrado." });
            }

            _context.Patrocinios.Remove(patrocinio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        private bool PatrocinioExists(int id)
        {
            return _context.Patrocinios.Any(e => e.Id == id);
        }
    }
}
