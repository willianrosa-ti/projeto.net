using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Context;
using PrimeiraApi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeiraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("login")]
        public async Task<ActionResult> GetLogin([FromQuery] string login, [FromQuery] string senha)
        {
            var usuarioEncontrado = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == login && u.Senha == senha);

            if (usuarioEncontrado == null)
            {
                return Unauthorized(new { message = "Login ou senha inválidos." });
            }

            return Ok(new { message = "Login realizado com sucesso!", usuarioEncontrado.Id });
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<Usuario>> PostRegistro(Usuario usuario)
        {
            var usuarioExistente = await _context.Usuarios.AnyAsync(u => u.Login == usuario.Login);

            if (usuarioExistente)
            {
                return BadRequest(new { message = "Este login já está em uso." });
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogin), new { id = usuario.Id }, usuario);
        }

        // Método PUT para edição de cadastro
        [HttpPut("{editarID}")]
        public async Task<ActionResult> PutUsuario(int editarID, [FromBody] Usuario usuario)
        {
            if (editarID != usuario.Id)
            {
                return BadRequest(new { message = "O ID da URL não corresponde ao ID do usuário." });
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usuarios.Any(e => e.Id == editarID))
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
    }
}
