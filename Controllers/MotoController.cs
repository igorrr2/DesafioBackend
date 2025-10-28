using DesafioBackend.Data;
using DesafioBackend.Events;
using DesafioBackend.Models;
using DesafioBackend.SolicitacaoRespostas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DesafioBackend.Controllers
{
    [ApiController]
    [Route("motos")]
    public class MotoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MotoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarMoto([FromBody] Moto solicitacao)
        {
            try
            {
                if (string.IsNullOrEmpty(solicitacao.Placa) || string.IsNullOrEmpty(solicitacao.Identificador)
                    || string.IsNullOrEmpty(solicitacao.Modelo) || solicitacao.Ano <= 0)
                {
                    return BadRequest(new RespostaGenerica("Dados inválidos"));
                }

                var existente = await _context.Motos
                    .FirstOrDefaultAsync(m => m.Placa == solicitacao.Placa);
                if (existente != null)
                    return BadRequest(new RespostaGenerica("Placa já cadastrada."));

                _context.Motos.Add(solicitacao);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObterMotoPorId), new { id = solicitacao.Id }, solicitacao);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterMotos([FromQuery] string? placa)
        {
            try
            {
                IQueryable<Moto> query = _context.Motos;

                if (!string.IsNullOrEmpty(placa))
                    query = query.Where(m => m.Placa == placa);

                var motos = await query.ToListAsync();

                return Ok(motos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}/placa")]
        public async Task<IActionResult> AtualizarPlaca(string id, AtualizarPlacaSolicitacao solicitacao)
        {
            try
            {
                var moto = await _context.Motos.FirstOrDefaultAsync(m => m.Identificador == id);
                if (moto == null)
                    return BadRequest(new RespostaGenerica("Dados inválidos"));

                var existente = await _context.Motos
                    .FirstOrDefaultAsync(m => m.Placa == solicitacao.Placa && m.Identificador != id);
                if (existente != null)
                    return BadRequest(new RespostaGenerica("Dados inválidos"));

                moto.Placa = solicitacao.Placa;
                await _context.SaveChangesAsync();

                return Ok(new RespostaGenerica("Placa modificada com sucesso"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterMotoPorId(string id)
        {
            try
            {
                var moto = await _context.Motos.FirstOrDefaultAsync(m => m.Identificador == id);
                if (moto == null)
                    return NotFound();

                return Ok(moto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarMoto(string id)
        {
            try
            {
                var moto = await _context.Motos.FirstOrDefaultAsync(m => m.Identificador == id);
                if (moto == null)
                    return NotFound(new RespostaGenerica("Moto não encontrada"));

                // Exemplo:
                // if (PossuiLocacoesAtivas(moto.Id))
                //     return BadRequest(new RespostaGenerica("Request mal formada"));

                _context.Motos.Remove(moto);
                await _context.SaveChangesAsync();

                var detalhes = new
                {
                    identificador = moto.Identificador,
                    ano = moto.Ano,
                    modelo = moto.Modelo,
                    placa = moto.Placa
                };

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
