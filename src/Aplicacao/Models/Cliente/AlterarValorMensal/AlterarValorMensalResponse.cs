namespace Aplicacao.Models.Cliente.AlterarValorMensal
{
    public sealed record AlterarValorMensalResponse(
    int ClienteId,
    decimal ValorMensalAnterior,
    decimal ValorMensalNovo,
    DateTime DataAlteracao,
    string Mensagem
    );  
}
