namespace Aplicacao.Models.Cesta
{
    public record CestaAtualResponse(
    int CestaId,
    string Nome,
    bool Ativa,
    DateTime DataCriacao,
    List<ItemAtualResponse> Itens
    );

    public record ItemAtualResponse(string Ticker, decimal Percentual, decimal CotacaoAtual);

}
