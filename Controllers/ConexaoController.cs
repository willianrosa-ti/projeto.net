using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Context;
using PrimeiraApi.Models;
using System.Threading.Tasks;
using System.Linq; 
using System;
using System.Collections.Generic; 

namespace PrimeiraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConexaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConexaoController(AppDbContext context)
        {
            _context = context;
        }

        // =================================================================================
        // 1. SOLICITAR CONEXÃO
        // =================================================================================
        [HttpPost("solicitar")]
        public async Task<ActionResult<Conexao>> SolicitarConexao([FromBody] SolicitacaoDto solicitacaoDto)
        {
            if (solicitacaoDto.SolicitanteId == solicitacaoDto.SolicitadoId)
            {
                return BadRequest(new { message = "Usuário não pode seguir a si mesmo." });
            }
            var conexaoExistente = await _context.Conexoes
                .FirstOrDefaultAsync(c => 
                    (c.SolicitanteId == solicitacaoDto.SolicitanteId && c.SolicitadoId == solicitacaoDto.SolicitadoId) ||
                    (c.SolicitanteId == solicitacaoDto.SolicitadoId && c.SolicitadoId == solicitacaoDto.SolicitanteId)
                );
            if (conexaoExistente != null)
            {
                return Conflict(new { message = "Uma conexão ou solicitação já existe.", status = conexaoExistente.Status });
            }
            var novaConexao = new Conexao
            {
                SolicitanteId = solicitacaoDto.SolicitanteId,
                SolicitadoId = solicitacaoDto.SolicitadoId,
                Status = StatusConexao.Pendente, 
                DataSolicitacao = DateTime.Now
            };
            _context.Conexoes.Add(novaConexao);
            await _context.SaveChangesAsync();
            return Ok(new StatusConexaoDto { Status = "Pendente", ConexaoId = novaConexao.Id });
        }
        
        // =================================================================================
        // 2. VERIFICAR STATUS
        // =================================================================================
        [HttpGet("status/{solicitanteId}/{solicitadoId}")]
        public async Task<ActionResult<StatusConexaoDto>> GetStatusConexao(int solicitanteId, int solicitadoId)
        {
             var conexao = await _context.Conexoes
                .FirstOrDefaultAsync(c =>
                    (c.SolicitanteId == solicitanteId && c.SolicitadoId == solicitadoId) ||
                    (c.SolicitanteId == solicitadoId && c.SolicitadoId == solicitanteId)
                );
            if (conexao == null)
            {
                return Ok(new StatusConexaoDto { Status = "Nenhum", ConexaoId = null });
            }
            string statusResult;
            if (conexao.Status == StatusConexao.Aceito)
            {
                statusResult = "Aceito"; 
            }
            else if (conexao.Status == StatusConexao.Recusado)
            {
                 statusResult = "Recusado"; 
            }
            else if (conexao.SolicitanteId == solicitanteId)
            {
                statusResult = "Pendente"; 
            }
            else
            {
                statusResult = "PendenteInverso";
            }
            return Ok(new StatusConexaoDto { Status = statusResult, ConexaoId = conexao.Id });
        }

        // =================================================================================
        // 3. ACEITAR SOLICITAÇÃO
        // =================================================================================
        [HttpPut("aceitar")]
        public async Task<IActionResult> AceitarConexao([FromBody] AcaoConexaoDto acaoDto)
        {
            var conexao = await _context.Conexoes.FindAsync(acaoDto.ConexaoId);
            if (conexao == null)
            {
                return NotFound(new { message = "Solicitação não encontrada." });
            }
            if (conexao.SolicitadoId != acaoDto.UsuarioLogadoId)
            {
                return Unauthorized(new { message = "Usuário não autorizado a aceitar esta solicitação." });
            }
            if (conexao.Status != StatusConexao.Pendente)
            {
                return BadRequest(new { message = "Esta solicitação não está mais pendente." });
            }
            conexao.Status = StatusConexao.Aceito;
            conexao.DataConfirmacao = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Conexão aceita." });
        }

        // =================================================================================
// 4. RECUSAR SOLICITAÇÃO (CORRIGIDO)
// =================================================================================
[HttpPut("recusar")]
public async Task<IActionResult> RecusarConexao([FromBody] AcaoConexaoDto acaoDto)
{
    var conexao = await _context.Conexoes.FindAsync(acaoDto.ConexaoId);

    if (conexao == null)
    {
        // Se já foi tratada, apenas retorne OK
        return Ok(new { message = "Solicitação não encontrada." });
    }

    // Validação de segurança
    if (conexao.SolicitadoId != acaoDto.UsuarioLogadoId)
    {
        return Unauthorized(new { message = "Ação não permitida." });
    }

    // 🟢 CORREÇÃO: Em vez de marcar como 'Recusado'giit, REMOVEMOS a solicitação.
    // Isto limpa o histórico e permite que o outro usuário envie um novo pedido no futuro.
    _context.Conexoes.Remove(conexao);
    
    await _context.SaveChangesAsync();
    
    return Ok(new { message = "Solicitação recusada e removida." });
}
        // =================================================================================
        // 5. LISTAR NOTIFICAÇÕES
        // =================================================================================
        [HttpGet("notificacoes/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<NotificacaoDto>>> GetNotificacoes(int usuarioId)
        {
            var notificacoes = await _context.Conexoes
                .Include(c => c.Solicitante) 
                .Where(c => c.SolicitadoId == usuarioId && c.Status == StatusConexao.Pendente)
                .OrderByDescending(c => c.DataSolicitacao)
                .Select(c => new NotificacaoDto
                {
                    ConexaoId = c.Id,
                    SolicitanteId = c.SolicitanteId,
                    SolicitanteNome = (c.Solicitante != null) ? c.Solicitante.Nome : "Usuário Desconhecido",
                    SolicitanteFotoUrl = (c.Solicitante != null) ? c.Solicitante.FotoUrl : null,
                    DataSolicitacao = c.DataSolicitacao
                })
                .ToListAsync();

            return Ok(notificacoes);
        }

        // =================================================================================
        // 6. LISTAR CONTATOS
        // =================================================================================
        [HttpGet("contatos/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<ContatoDto>>> GetContatos(int usuarioId)
        {
            var contatosSolicitados = await _context.Conexoes
                .Include(c => c.Solicitado) 
                .Where(c => c.SolicitanteId == usuarioId && c.Status == StatusConexao.Aceito)
                .Select(c => c.Solicitado)
                .ToListAsync();

            var contatosSolicitantes = await _context.Conexoes
                .Include(c => c.Solicitante) 
                .Where(c => c.SolicitadoId == usuarioId && c.Status == StatusConexao.Aceito)
                .Select(c => c.Solicitante)
                .ToListAsync();

            var todosContatos = contatosSolicitados
                .Concat(contatosSolicitantes) 
                .Where(u => u != null) 
                .Distinct() 
                .Select(u => new ContatoDto
                {
                    Id = u!.Id, 
                    Nome = u.Nome,
                    FotoUrl = u.FotoUrl
                })
                .ToList();

            return Ok(todosContatos);
        }
        
        // =================================================================================
        // 🟢 7. NOVO: DELETAR/ENCERRAR CONEXÃO
        // =================================================================================
        [HttpDelete("deletar/{id}")]
        public async Task<IActionResult> DeletarConexao(int id)
        {
            var conexao = await _context.Conexoes.FindAsync(id);

            if (conexao == null)
            {
                // Retornar OK/NotFound se a conexão já foi removida (melhor para idempotência)
                return NotFound(new { message = "Conexão não encontrada." });
            }

            // Ação: Deleta a conexão do banco de dados (Unfollow)
            _context.Conexoes.Remove(conexao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Conexão deletada com sucesso." });
        }
    }
}