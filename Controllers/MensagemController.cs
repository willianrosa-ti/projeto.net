using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeiraApi.Context;
using PrimeiraApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeiraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensagemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MensagemController(AppDbContext context)
        {
            _context = context;
        }

        // ... (Os métodos [HttpPost("solicitar")] não têm alterações) ...
        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarMensagem([FromBody] SolicitacaoMensagemDto dto)
        {
            if (dto.SolicitanteId == dto.SolicitadoId)
                return BadRequest(new { message = "Não pode enviar solicitação para si mesmo." });

            var conexao = await _context.Conexoes.FirstOrDefaultAsync(c =>
                ((c.SolicitanteId == dto.SolicitanteId && c.SolicitadoId == dto.SolicitadoId) ||
                 (c.SolicitanteId == dto.SolicitadoId && c.SolicitadoId == dto.SolicitanteId)) &&
                c.Status == StatusConexao.Aceito);

            if (conexao != null)
                return BadRequest(new { message = "Vocês já são contatos, não é necessário solicitar." });

            var solicitacaoExistente = await _context.SolicitacoesMensagem.FirstOrDefaultAsync(s =>
                ((s.SolicitanteId == dto.SolicitanteId && s.SolicitadoId == dto.SolicitadoId) ||
                 (s.SolicitanteId == dto.SolicitadoId && s.SolicitadoId == dto.SolicitanteId)) &&
                (s.Status == StatusSolicitacao.Pendente || s.Status == StatusSolicitacao.Aceito));

            if (solicitacaoExistente != null)
                return BadRequest(new { message = "Uma solicitação de mensagem já existe ou está pendente." });

            var novaSolicitacao = new SolicitacaoMensagem
            {
                SolicitanteId = dto.SolicitanteId,
                SolicitadoId = dto.SolicitadoId,
                Status = StatusSolicitacao.Pendente,
                DataSolicitacao = DateTime.Now
            };

            _context.SolicitacoesMensagem.Add(novaSolicitacao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitação de mensagem enviada com sucesso." });
        }


        // =================================================================================
        // 3. (Widget) NOTIFICAÇÕES (Req 3 - NotificationsWidget)
        // =================================================================================
        [HttpGet("notificacoes/{usuarioLogadoId}")]
        public async Task<ActionResult<IEnumerable<NotificacaoMensagemDto>>> GetNotificacoesMensagem(int usuarioLogadoId)
        {
            var notificacoes = await _context.SolicitacoesMensagem
                .Include(s => s.Solicitante) 
                .Where(s => s.SolicitadoId == usuarioLogadoId && s.Status == StatusSolicitacao.Pendente)
                .OrderByDescending(s => s.DataSolicitacao)
                .Select(s => new NotificacaoMensagemDto
                {
                    Id = s.Id,
                    SolicitanteId = s.SolicitanteId,
                    
                    // 🟢 CORREÇÃO 1: 
                    // Se s.Solicitante for nulo, usamos um valor padrão "Usuário Removido"
                    // (Isto assume que 'SolicitanteNome' no DTO (Data Transfer Object) é 'string' e não 'string?')
                    SolicitanteNome = s.Solicitante != null ? s.Solicitante.Nome : "Usuário Removido",
                    
                    // 🟢 CORREÇÃO 2: 
                    // Fazemos o mesmo para FotoUrl, retornando 'null' ou 'string.Empty'
                    // (Isto assume que 'SolicitanteFotoUrl' no DTO (Data Transfer Object) é 'string?')
                    SolicitanteFotoUrl = s.Solicitante != null ? s.Solicitante.FotoUrl : null,
                    
                    DataSolicitacao = s.DataSolicitacao
                })
                .ToListAsync();

            return Ok(notificacoes);
        }

        // ... (Os métodos [HttpPost("aceitar")] e [HttpPost("recusar")] não têm alterações) ...
        [HttpPost("aceitar")]
        public async Task<IActionResult> AceitarSolicitacao([FromBody] AcaoMensagemDto dto)
        {
            var solicitacao = await _context.SolicitacoesMensagem.FindAsync(dto.SolicitacaoId);

            if (solicitacao == null)
                return NotFound(new { message = "Solicitação não encontrada." });

            if (solicitacao.SolicitadoId != dto.UsuarioLogadoId)
                return Unauthorized(new { message = "Ação não permitida." });

            if (solicitacao.Status != StatusSolicitacao.Pendente)
                return BadRequest(new { message = "Esta solicitação não está mais pendente." });

            solicitacao.Status = StatusSolicitacao.Aceito;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitação de mensagem aceite." });
        }

        [HttpPost("recusar")]
        public async Task<IActionResult> RecusarSolicitacao([FromBody] AcaoMensagemDto dto)
        {
            var solicitacao = await _context.SolicitacoesMensagem.FindAsync(dto.SolicitacaoId);

            if (solicitacao == null)
                return NotFound(new { message = "Solicitação não encontrada." });

            if (solicitacao.SolicitadoId != dto.UsuarioLogadoId)
                return Unauthorized(new { message = "Ação não permitida." });

            _context.SolicitacoesMensagem.Remove(solicitacao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitação de mensagem recusada." });
        }


        // =================================================================================
        // 1. BUSCAR CONVERSAS (Req 1 - FloatingChat)
        // =================================================================================
        [HttpGet("conversas/{usuarioLogadoId}")]
        public async Task<ActionResult<IEnumerable<ConversaDto>>> GetConversas(int usuarioLogadoId)
        {
            var mensagens = await _context.Mensagens
                .Include(m => m.Remetente)
                .Include(m => m.Destinatario)
                .Where(m => m.RemetenteId == usuarioLogadoId || m.DestinatarioId == usuarioLogadoId)
                .OrderBy(m => m.DataEnvio) 
                .ToListAsync();

            var conversas = mensagens
                .GroupBy(m => (m.RemetenteId == usuarioLogadoId) ? m.DestinatarioId : m.RemetenteId)
                .Select(g =>
                {
                    var ultimoMsg = g.Last(); 
                    var outroUsuario = (ultimoMsg.RemetenteId == usuarioLogadoId) ? ultimoMsg.Destinatario : ultimoMsg.Remetente;

                    return new ConversaDto
                    {
                        OutroUsuarioId = g.Key,
                        
                        // 🟢 CORREÇÃO 3 (Profilática):
                        // Aplicando a mesma lógica aqui para evitar o mesmo erro.
                        OutroUsuarioNome = outroUsuario != null ? outroUsuario.Nome : "Usuário Desconhecido",
                        OutroUsuarioFotoUrl = outroUsuario != null ? outroUsuario.FotoUrl : null,
                        
                        UltimaMensagem = ultimoMsg.Conteudo,
                        DataUltimaMensagem = ultimoMsg.DataEnvio
                    };
                })
                .OrderByDescending(c => c.DataUltimaMensagem) 
                .ToList();

            return Ok(conversas);
        }

        // ... (O resto do ficheiro [HttpGet("historico")] etc. não tem alterações) ...
        [HttpGet("historico/{usuarioLogadoId}/{outroUsuarioId}")]
        public async Task<ActionResult<IEnumerable<MensagemDto>>> GetHistorico(int usuarioLogadoId, int outroUsuarioId)
        {
            var historico = await _context.Mensagens
                .Where(m => (m.RemetenteId == usuarioLogadoId && m.DestinatarioId == outroUsuarioId) ||
                            (m.RemetenteId == outroUsuarioId && m.DestinatarioId == usuarioLogadoId))
                .OrderBy(m => m.DataEnvio)
                .Select(m => new MensagemDto
                {
                    Id = m.Id,
                    RemetenteId = m.RemetenteId,
                    DestinatarioId = m.DestinatarioId,
                    Conteudo = m.Conteudo,
                    DataEnvio = m.DataEnvio
                })
                .ToListAsync();

            return Ok(historico);
        }

        [HttpPost("enviar")]
        public async Task<ActionResult<MensagemDto>> EnviarMensagem([FromBody] MensagemDto dto)
        {
            bool temPermissao = await VerificarPermissaoMensagem(dto.RemetenteId, dto.DestinatarioId);

            if (!temPermissao)
                return Unauthorized(new { message = "Você não tem permissão para enviar mensagem para este usuário." });

            var mensagem = new Mensagem
            {
                RemetenteId = dto.RemetenteId,
                DestinatarioId = dto.DestinatarioId,
                Conteudo = dto.Conteudo,
                DataEnvio = DateTime.Now,
                Lido = false
            };

            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();

            dto.Id = mensagem.Id;
            dto.DataEnvio = mensagem.DataEnvio;

            return CreatedAtAction(nameof(GetHistorico), new { usuarioLogadoId = dto.RemetenteId, outroUsuarioId = dto.DestinatarioId }, dto);
        }

        private async Task<bool> VerificarPermissaoMensagem(int usuarioA, int usuarioB)
        {
            var conexao = await _context.Conexoes.FirstOrDefaultAsync(c =>
                ((c.SolicitanteId == usuarioA && c.SolicitadoId == usuarioB) ||
                 (c.SolicitanteId == usuarioB && c.SolicitadoId == usuarioA)) &&
                c.Status == StatusConexao.Aceito);

            if (conexao != null) return true;

            var solicitacao = await _context.SolicitacoesMensagem.FirstOrDefaultAsync(s =>
                ((s.SolicitanteId == usuarioA && s.SolicitadoId == usuarioB) ||
                 (s.SolicitanteId == usuarioB && s.SolicitadoId == usuarioA)) &&
                s.Status == StatusSolicitacao.Aceito);
            
            if (solicitacao != null) return true;

            return false;
        }
    }
}