using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Cotacao
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public DateTime DataPregao { get; set; }
        public decimal PrecoFechamento { get; set; }
        public decimal PrecoAbertura { get; set; }
        public decimal PrecoMaximo { get; set; }
        public decimal PrecoMinimo { get; set; }
        public int TipoMercado { get; set; }
        public string CodigoBDI { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; }
    }
}
