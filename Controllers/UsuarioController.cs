using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Context;
using PrimeiraApi.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // NOVO: Necessário para IEnumerable<T>

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

        // --- GET Login (Mantido) ---
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

        // --- POST Registrar (Mantido) ---
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

            return CreatedAtAction("GetUsuarioPorId", new { id = usuario.Id }, usuario);
        }

        // --- PUT AtualizarFoto (Mantido) ---
        [HttpPut("atualizarfoto")]
        public async Task<IActionResult> PutAtualizarFoto([FromBody] FotoUsuarioDto fotoDto)
        {
            // Verifica se o usuário que está fazendo a requisição (Logado)
            // é o mesmo que está tentando atualizar.
            if (fotoDto.UsuarioId != fotoDto.UsuarioLogadoId)
            {
                // Se não for, retorna 401 Unauthorized (Não Autorizado).
                return Unauthorized(new { message = "Você não tem permissão para atualizar este perfil." });
            }

            var usuario = await _context.Usuarios.FindAsync(fotoDto.UsuarioId);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            // Atualiza a URL (Uniform Resource Locator) da foto de perfil
            usuario.FotoUrl = fotoDto.FotoUrl;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Erro ao salvar a foto de perfil." });
            }

            return NoContent(); // Retorna 204 Sucesso sem conteúdo
        }
        
        // --- GET por ID (Mantido) ---
        [HttpGet("{id}")] 
        public async Task<ActionResult<Usuario>> GetUsuarioPorId(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }
        
            return Ok(usuario);
        }

        // =================================================================
        // 🟢 NOVO: ENDPOINT DE BUSCA (CORRIGIDO PARA O 400 BAD REQUEST)
        // =================================================================
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ContatoDto>>> BuscarUsuarios([FromQuery] string? query) // string? faz o parâmetro ser opcional
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok(new List<ContatoDto>());
            }
            
            // Usa ToLower() para busca case-insensitive
            var lowerQuery = query!.ToLower();

            var usuarios = await _context.Usuarios
                // Filtra usuários cujo Nome ou Login contenham a query
                .Where(u => u.Nome.ToLower().Contains(lowerQuery) || u.Login.ToLower().Contains(lowerQuery)) 
                .Take(10) // Limita a 10 resultados para otimizar
                .Select(u => new ContatoDto // Mapeia para o DTO (Objeto de Transferência de Dados) simplificado
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    FotoUrl = u.FotoUrl
                })
                .ToListAsync();

            return Ok(usuarios);
        }
    }
}