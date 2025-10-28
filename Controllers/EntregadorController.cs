using DesafioBackend.Data;
using DesafioBackend.Models;
using DesafioBackend.SolicitacaoRespostas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DesafioBackend.Controllers
{
    [ApiController]
    [Route("entregadores")]
    public class EntregadorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string CaminhoStorage = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "Cnhs");

        public EntregadorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarEntregador([FromBody] Entregador solicitacao)
        {
            if (string.IsNullOrEmpty(solicitacao.Identificador) ||
                string.IsNullOrEmpty(solicitacao.Nome) ||
                string.IsNullOrEmpty(solicitacao.Cnpj) ||
                string.IsNullOrEmpty(solicitacao.NumeroCNH) ||
                string.IsNullOrEmpty(solicitacao.TipoCNH) ||
                string.IsNullOrEmpty(solicitacao.ImagemCNH))
            {
                return BadRequest(new RespostaGenerica("Dados inválidos."));
            }

            try
            {
                var tiposValidos = new[] { "A", "B", "A+B" };
                if (!tiposValidos.Contains(solicitacao.TipoCNH.ToUpper()))
                {
                    return BadRequest(new RespostaGenerica("Tipo de CNH inválido. Tipos válidos: A, B ou A+B."));
                }

                bool cnpjExistente = await _context.Entregadores.AnyAsync(e => e.Cnpj == solicitacao.Cnpj);
                if (cnpjExistente)
                    return BadRequest(new RespostaGenerica("CNPJ já cadastrado."));

                bool cnhExistente = await _context.Entregadores.AnyAsync(e => e.NumeroCNH == solicitacao.NumeroCNH);
                if (cnhExistente)
                    return BadRequest(new RespostaGenerica("CNH já cadastrada."));

                _context.Entregadores.Add(solicitacao);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/cnh")]
        public async Task<IActionResult> AtualizarImagemCnh(string id, [FromBody] AtualizarImagemCnhSolicitacao solicitacao)
        {
            if (solicitacao == null || string.IsNullOrEmpty(solicitacao.ImagemCnh))
                return BadRequest(new RespostaGenerica("Dados inválidos."));

            try
            {
                var entregador = await _context.Entregadores
                .FirstOrDefaultAsync(e => e.Identificador == id);


                byte[] imagemBytes = Convert.FromBase64String(solicitacao.ImagemCnh);

                if (!Directory.Exists(CaminhoStorage))
                    Directory.CreateDirectory(CaminhoStorage);

                Mensagem mensagem = Util.RemoverAcentos(entregador.Nome, out string nomeSemAcento);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                var nomeArquivo = $"cnh_{nomeSemAcento.ToLower().Trim().Replace(" ", "_")}.png";
                var caminhoCompleto = Path.Combine(CaminhoStorage, nomeArquivo);

                await System.IO.File.WriteAllBytesAsync(caminhoCompleto, imagemBytes);

                entregador.ImagemCNH = caminhoCompleto;
                _context.Entregadores.Update(entregador);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (FormatException)
            {
                return BadRequest(new RespostaGenerica("Formato inválido. A imagem deve estar em Base64."));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
