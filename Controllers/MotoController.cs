using DesafioBackend.Data;
using DesafioBackend.Models;
using DesafioBackend.SolicitacaoRespostas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            if(string.IsNullOrEmpty(solicitacao.Placa) || string.IsNullOrEmpty(solicitacao.Identificador) || string.IsNullOrEmpty(solicitacao.Modelo) || solicitacao.Ano <= 0)
            {
                RespostaGenerica resposta = new RespostaGenerica("Dados inválidos");
                return BadRequest(resposta);
            }
            // Verificar se a placa já existe
            var existente = await _context.Motos
                .FirstOrDefaultAsync(m => m.Placa == solicitacao.Placa);

            if (existente != null)
                return BadRequest("Placa já cadastrada.");

            _context.Motos.Add(solicitacao);
            await _context.SaveChangesAsync();

            // Retorna 201 Created com a moto cadastrada
            return CreatedAtAction(nameof(ObterMotoPorId), new { id = solicitacao.Id }, solicitacao);
        }

        [HttpGet]
        public async Task<IActionResult> ObterMotos([FromQuery] string? placa)
        {
            IQueryable<Moto> query = _context.Motos;

            if (!string.IsNullOrEmpty(placa))
                query = query.Where(m => m.Placa == placa);

            var motos = await query.ToListAsync();

            return Ok(motos);
        }
        

        [HttpPut("{id}/placa")]
        public async Task<IActionResult> AtualizarPlaca(int id, AtualizarPlacaSolicitacao solicitacao)
        {
            // Busca a moto pelo Id
            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return BadRequest(new RespostaGenerica("Dados inválidos"));

            // Verifica se a nova placa já está cadastrada
            var existente = await _context.Motos
                .FirstOrDefaultAsync(m => m.Placa == solicitacao.Placa && m.Id != id);
            if (existente != null)
                return BadRequest(new RespostaGenerica("Dados inválidos"));

            // Atualiza a placa
            moto.Placa = solicitacao.Placa;
            await _context.SaveChangesAsync();

            return Ok(new RespostaGenerica("Placa modificada com sucesso"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterMotoPorId(int id)
        {
            var moto = await _context.Motos.FirstOrDefaultAsync(m => m.Id == id);
            if (moto == null)
                return NotFound();

            return Ok(moto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarMoto(int id)
        {
            if (id <= 0)
                return BadRequest(new RespostaGenerica("Request mal formada"));

            var moto = await _context.Motos.FindAsync(id);
            if (moto == null)
                return NotFound(new RespostaGenerica("Moto não encontrada"));

            // Se houver checagem de locações ativas, colocar aqui
            // Exemplo:
            // if (PossuiLocacoesAtivas(moto.Id))
            //     return BadRequest(new RespostaGenerica("Request mal formada"));

            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            // Retorna os detalhes da moto apagada conforme Swagger
            var detalhes = new
            {
                identificador = moto.Identificador,
                ano = moto.Ano,
                modelo = moto.Modelo,
                placa = moto.Placa
            };

            return Ok();
        }
    }
}
