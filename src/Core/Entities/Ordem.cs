using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    
    public class Ordem
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = null!;
        public int QuantidadeTotal { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public string TipoOrdem { get; set; } = null!;
        public DateTime DataCompra { get; set; }


        public virtual List<DetalheOrdem> Detalhes { get; set; } = new List<DetalheOrdem>();
    }

    public class DetalheOrdem
    {
        [Key]
        public int Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }

        public int OrdemId { get; set; }
        [ForeignKey("OrdemId")]
        public virtual Ordem Ordem { get; set; } = null!;
    }

    public class ResiduoMaster
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }

        public int? OrdemId { get; set; }
        [ForeignKey("OrdemId")]
        public virtual Ordem? OrdemOrigem { get; set; }
    }

    public class DistribuicaoCliente
    {
        [Key]
        public int Id { get; set; } 
        public int ClienteId { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime DataCriacao { get; set; }
        public decimal ValorAporte { get; set; }
        public virtual List<AtivoDistribuido> Ativos { get; set; } = new List<AtivoDistribuido>();
    }

    public class AtivoDistribuido
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }
        public int DistribuicaoClienteId { get; set; }
        [ForeignKey("DistribuicaoClienteId")]
        public virtual DistribuicaoCliente DistribuicaoCliente { get; set; } = null!;
    }
}
