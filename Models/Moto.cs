using DesafioBackend.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DesafioBackend.Models
{

    public class Moto
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Identificador { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;

        public Moto() { }

        public Moto(string identificador, int ano, string modelo, string placa)
        {
            Identificador = identificador;
            Ano = ano;
            Modelo = modelo;
            Placa = placa;
        }

        public static async Task<Mensagem> TryAdd(AppDbContext context, Moto moto)
        {
            try
            {
                context.Motos.Add(moto);
                await context.SaveChangesAsync();

                return new Mensagem();
            }
            catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }

        public static async Task<(Mensagem, Moto)> TryGetById(AppDbContext context, string identificador)
        {
            try
            {
                Moto moto = await context.Motos.FirstOrDefaultAsync(e => e.Identificador == identificador);

                return (new Mensagem(), moto);
            }
            catch (Exception ex)
            {
                return (new Mensagem(ex.Message, ex), null);
            }
        }

        public static async Task<(Mensagem, Moto)> TryGetByPlaca(AppDbContext context, string placa)
        {
            try
            {
                Moto moto = await context.Motos
                   .FirstOrDefaultAsync(m => m.Placa == placa);

                return (new Mensagem(), moto);
            }
            catch (Exception ex)
            {
                return (new Mensagem(ex.Message, ex), null);
            }
        }

        public static async Task<Mensagem> TryUpdate(AppDbContext context, Moto moto)
        {
            try
            {
                context.Motos.Update(moto);
                await context.SaveChangesAsync();

                return new Mensagem();
            }
            catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }

        public static async Task<Mensagem> TryDelete(AppDbContext context, Moto moto)
        {
            try
            {
                context.Motos.Remove(moto);
                await context.SaveChangesAsync();

                return new Mensagem();
            }
            catch (Exception ex)
            {
                return new Mensagem(ex.Message, ex);
            }
        }

        public static bool PossuiLocacoesAtivas(AppDbContext context, string motoId)
        {
            return context.Locacoes.Any(l => l.MotoId == motoId && DateTime.Now < l.DataFim);
        }
    }
}
