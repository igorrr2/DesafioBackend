using DesafioBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DesafioBackend.Models
{
    public class Locacao
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Identificador { get; set; }
        public string EntregadorId { get; set; }

        public string MotoId { get; set; }

        public int PlanoDias { get; set; }
        public decimal ValorDiaria { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime DataPrevistaFim { get; set; }
        public DateTime DataFim { get; set; }

        public decimal ValorTotal { get; set; }
        public decimal Multa { get; set; }
        public static async Task<Mensagem> TryAdd(AppDbContext context, Locacao locacao)
        {
            try
            {
                context.Locacoes.Add(locacao);
                await context.SaveChangesAsync();

                return new Mensagem();
            }
            catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }

        public static async Task<(Mensagem, Locacao)> TryGetById(AppDbContext context, string identificador)
        {
            try
            {
                Locacao locacao = await context.Locacoes
                .FirstOrDefaultAsync(l => l.Identificador == identificador);

                return (new Mensagem(), locacao);
            }
            catch (Exception ex)
            {
                return (new Mensagem(ex.Message, ex), null);
            }
        }
        public static async Task<Mensagem> TryUpdate(AppDbContext context, Locacao locacao)
        {
            try
            {
                context.Locacoes.Update(locacao);
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
