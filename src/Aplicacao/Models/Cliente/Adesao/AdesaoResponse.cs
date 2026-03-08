namespace Aplicacao.Models.Cliente.Adesao
{
    public sealed record AdesaoResponse(
    int ClienteId,
    string Nome,
    string Cpf,
    string Email,
    decimal ValorMensal,
    bool Ativo,
    DateTime DataAdesao,
    ContaGraficaResponse ContaGrafica
    );
}
