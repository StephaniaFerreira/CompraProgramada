namespace Cliente.Models.Saida
{
    public sealed record SaidaResponse(
    int ClienteId,
    string Nome,
    bool Ativo,
    DateTime DataSaida,
    string Mensagem
);
}
