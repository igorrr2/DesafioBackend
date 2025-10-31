using DesafioBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DesafioBackend.Models
{
    public class Entregador
    {
        [JsonIgnore]

        public int Id { get; set; }

        public string Identificador { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Cnpj { get; set; } = string.Empty;

        public DateTime DataNascimento { get; set; }

        public string NumeroCNH { get; set; } = string.Empty;

        public string TipoCNH { get; set; } = string.Empty;

        public string ImagemCNH { get; set; } = string.Empty;

        public Entregador()
        {

        }

        public Entregador(EntregadorSolicitacao solicitacao)
        {
            this.Identificador = solicitacao.Identificador;
            this.Cnpj = solicitacao.Cnpj;
            this.DataNascimento = solicitacao.DataNascimento;
            this.NumeroCNH = solicitacao.NumeroCNH;
            this.TipoCNH = solicitacao.TipoCNH;
            this.Nome = solicitacao.Nome;
            this.ImagemCNH = solicitacao.ImagemCNH;
            this.DataNascimento = solicitacao.DataNascimento;
        }

        public static async Task<Mensagem> TryAdd(AppDbContext context, Entregador entregador)
        {
            try
            {
                context.Entregadores.Add(entregador);
                await context.SaveChangesAsync();

                return new Mensagem();
            }
            catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }

        public static async Task<(Mensagem, Entregador)> TryGetById(AppDbContext context, string identificador)
        {
            try
            {
                Entregador entregador = await context.Entregadores
                    .FirstOrDefaultAsync(e => e.Identificador == identificador);

                return (new Mensagem(), entregador);
            }
            catch (Exception ex)
            {
                return (new Mensagem(ex.Message, ex), null);
            }
        }

        public static async Task<Mensagem> TryUpdate(AppDbContext context, Entregador entregador)
        {
            try
            {
                context.Entregadores.Update(entregador);
                await context.SaveChangesAsync();

                return new Mensagem();
            }
            catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }
    }
}
