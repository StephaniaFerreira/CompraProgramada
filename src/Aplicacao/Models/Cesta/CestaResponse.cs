namespace Aplicacao.Models.Cesta
{
    public record CestaResponse(
    int CestaId,
    string Nome,
    bool Ativa,
    DateTime DataCriacao,
    List<ItemResponse> Itens,
    bool RebalanceamentoDisparado,
    string? Mensagem,
    DateTime? DataDesativacao = null,
    CestaAnteriorResponse? CestaAnteriorDesativada = null,
    List<string>? AtivosRemovidos = null,
    List<string>? AtivosAdicionados = null
    );

    public record ItemResponse(string Ticker, decimal Percentual);

    public record CestaAnteriorResponse(int Id, string Nome, DateTime DataDesativacao);


}
