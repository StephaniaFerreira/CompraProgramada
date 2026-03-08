namespace Aplicacao.Models.Cliente.Adesao
{
    public sealed record ContaGraficaResponse(
    int Id,
    string NumeroConta,
    string Tipo,
    DateTime DataCriacao
    );
}
