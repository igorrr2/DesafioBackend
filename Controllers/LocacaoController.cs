using DesafioBackend.Data;
using DesafioBackend.Models;
using DesafioBackend.SolicitacaoRespostas;
using DesafioBackend.SolicitacaoRespostas.Locacao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DesafioBackend.Controllers
{
    [ApiController]
    [Route("locacao")]
    public class LocacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocacaoController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CriarLocacao([FromBody] CriarLocacaoSolicitacao solicitacao)
        {
            try
            {
                var entregador = await _context.Entregadores.FirstOrDefaultAsync(e => e.Identificador == solicitacao.EntregadorId);
                if (entregador == null)
                    return NotFound();

                if (entregador.TipoCNH.ToUpper() != "A")
                    return BadRequest("Apenas entregadores com CNH categoria A podem alugar.");

                if (!Enum.IsDefined(typeof(PlanoLocacao), solicitacao.Plano))
                    return BadRequest("Plano inválido.");

                var planoEnum = (PlanoLocacao)solicitacao.Plano;

                decimal valorDiaria = PlanoLocacaoInfo.ObterValorDiaria(planoEnum);
                int dias = (int)planoEnum;
                decimal valorTotal = dias * valorDiaria;

                var locacao = new Locacao
                {
                    EntregadorId = solicitacao.EntregadorId,
                    Identificador = solicitacao.Identificador,
                    MotoId = solicitacao.MotoId,
                    PlanoDias = dias,
                    ValorDiaria = valorDiaria,
                    DataInicio = solicitacao.DataInicio,
                    DataPrevistaFim = solicitacao.DataPrevisaoTermino,
                    DataFim = solicitacao.DataTermino,
                    ValorTotal = valorTotal
                };

                _context.Locacoes.Add(locacao);
                await _context.SaveChangesAsync();

                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]

        public async Task<IActionResult> ObterLocacaoPorIdentificador(string id)
        {
            try
            {
                var locacao = await _context.Locacoes
                    .FirstOrDefaultAsync(l => l.Identificador == id);

                if (locacao == null)
                    return NotFound(new { mensagem = "Locação não encontrada." });

                var resposta = new
                {
                    locacao.Identificador,
                    valor_diaria = locacao.ValorDiaria,
                    entregador_id = locacao.EntregadorId,
                    moto_id = locacao.MotoId,
                    data_inicio = locacao.DataInicio,
                    data_termino = locacao.DataFim,
                    data_previsao_termino = locacao.DataPrevistaFim,
                    data_devolucao = locacao.DataFim
                };

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{id}/devolucao")]
        public async Task<IActionResult> DevolverLocacao([FromRoute] string id, [FromBody] DevolucaoSolicitacao solicitacao)
        {
            try
            {
                if (solicitacao == null || solicitacao.DataDevolucao == null)
                    return BadRequest(new { mensagem = "Data de devolução inválida." });

                var locacao = await _context.Locacoes.FirstOrDefaultAsync(l => l.Identificador == id);

                if (locacao == null)
                    return NotFound(new RespostaGenerica("Locação não encontrada."));

                locacao.DataFim = solicitacao.DataDevolucao;
                var planoEnum = (PlanoLocacao)locacao.PlanoDias;

                if (locacao.DataFim < locacao.DataPrevistaFim)
                {
                    int diasNaoUsados = (locacao.DataPrevistaFim - locacao.DataFim).Days;
                    decimal percentualMulta = PlanoLocacaoInfo.ObterMulta(planoEnum);
                    locacao.Multa = diasNaoUsados * locacao.ValorDiaria * percentualMulta;
                }
                else if (locacao.DataFim > locacao.DataPrevistaFim)
                {
                    int diasExtras = (int)Math.Ceiling((locacao.DataFim - locacao.DataPrevistaFim).TotalDays);
                    locacao.Multa = diasExtras * 50;
                }

                locacao.ValorTotal += locacao.Multa;

                await _context.SaveChangesAsync();

                return Ok(new RespostaGenerica("Data de devolução informada com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}
