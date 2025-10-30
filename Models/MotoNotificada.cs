namespace DesafioBackend.Models
{
    public class MotoNotificada
    {
        public int Id { get; set; }
        public string Identificador { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public DateTime DataNotificacao { get; set; }
    }
}
