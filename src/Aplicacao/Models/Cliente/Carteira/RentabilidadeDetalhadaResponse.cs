namespace Aplicacao.Models.Cliente.Carteira
{
    public record RentabilidadeResumo(
    decimal ValorTotalInvestido,
    decimal ValorAtualCarteira,
    decimal PlTotal,
    decimal RentabilidadePercentual
);

    public record HistoricoAporte(
        string Data,
        decimal Valor,
        string Parcela
    );

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
