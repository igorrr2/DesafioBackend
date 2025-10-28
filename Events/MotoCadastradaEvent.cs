namespace DesafioBackend.Events
{
    public class MotoCadastradaEvent
    {
        public string Identificador { get; set; }   
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
    }
}
