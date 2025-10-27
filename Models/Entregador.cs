namespace DesafioBackend.Models
{
    public class Entregador
    {
        public int Id { get; set; }
        public string Identificador { get; set; } = Guid.NewGuid().ToString();
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string NumeroCnh { get; set; } = string.Empty;
        public string TipoCnh { get; set; } = string.Empty;
        public string? CaminhoImagemCnh { get; set; }
    }
}
