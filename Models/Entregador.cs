using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DesafioBackend.Models
{
    public class Entregador
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [JsonPropertyName("identificador")]
        public string Identificador { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("data_nascimento")]
        public DateTime DataNascimento { get; set; }

        [JsonPropertyName("numero_cnh")]
        public string NumeroCNH { get; set; } = string.Empty;

        [JsonPropertyName("tipo_cnh")]
        public string TipoCNH { get; set; } = string.Empty;

        [JsonPropertyName("imagem_cnh")]
        public string ImagemCNH { get; set; } = string.Empty;
    }
}
