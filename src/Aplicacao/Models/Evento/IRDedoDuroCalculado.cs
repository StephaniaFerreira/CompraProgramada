namespace Aplicacao.Models.Evento
{
    public class IRDedoDuroCalculado
    {
        public int ClienteId { get; set; }
        public string CPF { get; set; } = null!;
        public string Ticker { get; set; } = null!;
        public decimal ValorOperacao { get; set; }
        public decimal ValorImposto { get; set; }
        public DateTime Data { get; set; }
    }
}

