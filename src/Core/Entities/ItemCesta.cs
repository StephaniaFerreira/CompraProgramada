using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ItemCesta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ticker { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Percentual { get; set; }

        public decimal? CotacaoAtual { get; set; }

        public int CestaId { get; set; }

        public Cesta Cesta { get; set; } = null!;
    }
}
