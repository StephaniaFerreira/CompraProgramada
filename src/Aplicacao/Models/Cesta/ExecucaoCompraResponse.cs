using Core.Entities;

namespace Aplicacao.Models.Cesta
{
    public class ExecucaoCompraResponse
    {
        public DateTime DataExecucao { get; set; } 
        public int TotalClientes { get; set; } 
        public decimal TotalConsolidado { get; set; } 

        public List<Ordem> Ordens { get; set; } = new List<Ordem>();

        
        public List<DistribuicaoCliente> Distribuicoes { get; set; } = new List<DistribuicaoCliente>();

        
        public List<ResiduoMaster> ResiduosCustMaster { get; set; } = new List<ResiduoMaster>();

        public int EventosIRPublicados { get; set; }
        public string Mensagem { get; set; } = null!;
    }
}
