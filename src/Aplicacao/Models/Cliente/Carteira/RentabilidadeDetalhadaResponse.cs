namespace Aplicacao.Models.Cliente.Carteira
{
    public record RentabilidadeResumo(
    decimal ValorTotalInvestido,
    decimal ValorAtualCarteira,
    decimal PlTotal,
    decimal RentabilidadePercentual
);

    public class HistoricoAporte {
        public string Data { get; set; }
        public decimal Valor { get; set; }
        public string Parcela { get; set; } = null!;
    }

    public record EvolucaoCarteira(
        string Data,
        decimal ValorCarteira,
        decimal ValorInvestido,
        decimal Rentabilidade
    );

    public record RentabilidadeDetalhadaResponse(
        int ClienteId,
        string Nome,
        DateTime DataConsulta,
        RentabilidadeResumo Rentabilidade,
        List<HistoricoAporte> HistoricoAportes,
        List<EvolucaoCarteira> EvolucaoCarteira
    );
}
