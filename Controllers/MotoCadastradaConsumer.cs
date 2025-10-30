using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;
using DesafioBackend.Events;
using DesafioBackend.Data;
using DesafioBackend.Models;

namespace DesafioBackend.Services
{
    public class MotoCadastradaConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MotoCadastradaConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            IConnection conn = await factory.CreateConnectionAsync();
            IChannel channel = await conn.CreateChannelAsync();

            await channel.QueueDeclareAsync("moto_cadastrada_queue", false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var evento = JsonConvert.DeserializeObject<MotoCadastradaEvent>(message);

                if (evento != null && evento.Ano == 2024)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var registro = new MotoNotificada
                    {
                        Identificador = evento.Identificador,
                        Ano = evento.Ano,
                        Modelo = evento.Modelo,
                        Placa = evento.Placa,
                        DataNotificacao = evento.DataCadastro
                    };

                    context.MotosNotificadas.Add(registro);
                    try
                    {
                        await context.SaveChangesAsync();
                    }catch(Exception ex)
                    {
                        string teste;
                    }
                }

                channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsumeAsync(queue: "moto_cadastrada_queue", autoAck: false, consumer: consumer);

            // Mantém o serviço rodando até o cancelamento
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
