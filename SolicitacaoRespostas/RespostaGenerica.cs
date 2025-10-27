using System.Text.Json.Serialization;

namespace DesafioBackend.SolicitacaoRespostas
{
    public class RespostaGenerica
    {
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; } = string.Empty;
        public RespostaGenerica(string mensagem)
        {
            Mensagem = mensagem;
        }
    }
}
