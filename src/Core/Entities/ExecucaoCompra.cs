using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    
    public class OrdemCompra
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = null!;
        public int QuantidadeTotal { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal { get; set; }


        public virtual List<DetalheOrdem> Detalhes { get; set; } = new List<DetalheOrdem>();
    }

    public class DetalheOrdem
    {
        [Key]
        public int Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }

        public int OrdemCompraId { get; set; }
        [ForeignKey("OrdemCompraId")]
        public virtual OrdemCompra OrdemCompra { get; set; } = null!;
    }

    public class ResiduoMaster
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }

        public int? OrdemCompraId { get; set; }
        [ForeignKey("OrdemCompraId")]
        public virtual OrdemCompra? OrdemOrigem { get; set; }
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
