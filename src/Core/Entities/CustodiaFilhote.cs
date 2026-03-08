using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class CustodiaFilhote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContaGraficaId { get; set; }
        public virtual ContaGrafica ContaGrafica { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Ticker { get; set; } = null!;

        [Required]
        public int Quantidade { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecoMedio { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAtual { get; set; } 

        public DateTime DataUltimaAtualizacao { get; set; }
    }
}

