namespace Aplicacao.Models.Cliente.Carteira
{
    public record CarteiraResponse(
     int ClienteId,
     string Nome,
     string ContaGrafica,
     DateTime DataConsulta,
     ResumoCarteira Resumo,
     List<AtivoCarteira> Ativos
    );

    public record AtivoCarteira(
    string Ticker,
    int Quantidade,
    decimal PrecoMedio,
    decimal CotacaoAtual,
    decimal ValorAtual,
    decimal Pl,
    decimal PlPercentual,
    decimal ComposicaoCarteira
    );

    public record ResumoCarteira(
        decimal ValorTotalInvestido,
        decimal ValorAtualCarteira,
        decimal PlTotal,
        decimal RentabilidadePercentual
    );
}



