using System.Text.Json.Serialization;

namespace DesafioBackend.SolicitacaoRespostas
{
    public class AtualizarImagemCnhSolicitacao
    {
        [JsonPropertyName("imagem_cnh")]
        public string ImagemCnh { get; set; } = string.Empty;
    }
}
