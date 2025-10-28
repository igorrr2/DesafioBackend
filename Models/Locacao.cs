namespace DesafioBackend.Models
{
    public class Locacao
    {
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
    }

}
