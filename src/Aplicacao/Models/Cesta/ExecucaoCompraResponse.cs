using Core.Entities;

namespace Aplicacao.Models.Cesta
{
    public class ExecucaoCompraResponse
    {
        public DateTime DataExecucao { get; set; } 
        public int TotalClientes { get; set; } 
        public decimal TotalConsolidado { get; set; } 
        public List<OrdemResponse> OrdensCompra { get; set; } = new List<OrdemResponse>();
        public List<DistribuicaoClienteResponse> Distribuicoes { get; set; } = new List<DistribuicaoClienteResponse>();
        public List<ResiduoMasterResponse> ResiduosCustMaster { get; set; } = new List<ResiduoMasterResponse>();
        public int EventosIRPublicados { get; set; }
        public string Mensagem { get; set; } = null!;
    }

    public class OrdemResponse
    {
        public string Ticker { get; set; } = null!;
        public int QuantidadeTotal { get; set; }
        public List<DetalheOrdemResponse> Detalhes { get; set; } = new();
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class DetalheOrdemResponse
    {
        public string Tipo { get; set; } = null!;
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }
    }
    public class DistribuicaoClienteResponse
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; } = null!;
        public decimal ValorAporte { get; set; }
        public virtual List<AtivoDistribuidoResponse> Ativos { get; set; } = new List<AtivoDistribuidoResponse>();
    }
    public class AtivoDistribuidoResponse
    {
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }
    }

    public class ResiduoMasterResponse
    {
        public string Ticker { get; set; } = null!;
        public int Quantidade { get; set; }
    }
}
