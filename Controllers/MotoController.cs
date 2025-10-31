using DesafioBackend.Data;
using DesafioBackend.Events;
using DesafioBackend.Models;
using DesafioBackend.SolicitacaoRespostas;
using DesafioBackend.SolicitacaoRespostas.Moto;
using DesafioBackend.SolicitacaoRespostas.Placa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public async Task<IActionResult> CadastrarMoto([FromBody] MotoSolicitacao solicitacao)
        {
            Mensagem mensagem = new Mensagem();
            Moto motoExistente = new Moto();
            Moto motoNova = new Moto(solicitacao.Identificador, solicitacao.Ano, solicitacao.Modelo, solicitacao.Placa);
            try
            {
                if (string.IsNullOrEmpty(solicitacao.Placa) || string.IsNullOrEmpty(solicitacao.Identificador)
                    || string.IsNullOrEmpty(solicitacao.Modelo) || solicitacao.Ano <= 0)
                {
                    return BadRequest(new RespostaGenerica("Dados inválidos"));
                }

                (mensagem, motoExistente) = await Moto.TryGetByPlaca(_context, solicitacao.Placa);

                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                if (motoExistente != null)
                    return BadRequest(new RespostaGenerica("Placa já cadastrada."));

                mensagem = await Moto.TryAdd(_context, motoNova);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
                IConnection conn = await factory.CreateConnectionAsync();
                IChannel channel = await conn.CreateChannelAsync();

                await channel.QueueDeclareAsync("moto_cadastrada_queue", false, false, false, null);

                var evento = new MotoCadastradaEvent
                {
                    Identificador = solicitacao.Identificador,
                    Ano = solicitacao.Ano,
                    Modelo = solicitacao.Modelo,
                    Placa = solicitacao.Placa,
                };

                var message = JsonConvert.SerializeObject(evento);
                var body = Encoding.UTF8.GetBytes(message);
                var props = new BasicProperties();

                await channel.BasicPublishAsync(string.Empty, "moto_cadastrada_queue", false, props, body);

                return Ok(motoNova);
            }
            catch (Exception ex)
            {
                return BadRequest(new RespostaGenerica(ex.Message + "\n\n" + ex.StackTrace));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterMotos([FromQuery] string? placa)
        {
            Mensagem mensagem = new Mensagem();
            Moto moto = new Moto();
            try
            {

                (mensagem, moto) = await Moto.TryGetByPlaca(_context, placa);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                if (moto == null)
                    return BadRequest(new RespostaGenerica("Moto não encontrada"));

                return Ok(moto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}/placa")]
        public async Task<IActionResult> AtualizarPlaca(string id, AtualizarPlacaSolicitacao solicitacao)
        {
            Mensagem mensagem = new Mensagem();
            Moto moto = new Moto();
            Moto motoExistente = new Moto();
            try
            {
                (mensagem, moto) = await Moto.TryGetById(_context, id);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                if (moto == null)
                    return BadRequest(new RespostaGenerica("Dados inválidos"));

                (mensagem, motoExistente) = await Moto.TryGetByPlaca(_context, id);

                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                if (motoExistente != null && motoExistente.Identificador != id)
                    return BadRequest(new RespostaGenerica("Dados inválidos"));

                moto.Placa = solicitacao.Placa;

                mensagem = await Moto.TryUpdate(_context, moto);

                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

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
            Mensagem mensagem = new Mensagem();
            Moto moto = new Moto();
            try
            {
                (mensagem, moto) = await Moto.TryGetById(_context, id);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                if (moto == null)
                    return BadRequest(new RespostaGenerica("Moto não encontrada"));

                return Ok(moto);
            }
            catch (Exception ex)
            {
                return BadRequest(new RespostaGenerica(ex.Message + "\n\n" + ex.StackTrace));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ApagarMoto(string id)
        {
            Mensagem mensagem = new Mensagem();
            Moto moto = new Moto();
            try
            {

                (mensagem, moto) = await Moto.TryGetById(_context, id);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                if (moto == null)
                    return BadRequest(new RespostaGenerica("Moto não encontrada"));

                if (Moto.PossuiLocacoesAtivas(_context, moto.Identificador))
                    return BadRequest(new RespostaGenerica("Não é possível apagar a moto pois possui locação ativa"));


                mensagem = await Moto.TryDelete(_context, moto);
                if (!mensagem.Sucesso)
                    return BadRequest(new RespostaGenerica(mensagem.Descricao));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
