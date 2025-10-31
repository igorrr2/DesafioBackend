using System.Text.Json.Serialization;

namespace DesafioBackend.SolicitacaoRespostas.Placa
{
    public class AtualizarPlacaSolicitacao
    {
        [JsonPropertyName("placa")]
        public string Placa { get; set; } = string.Empty;
    }
}