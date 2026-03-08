using Aplicacao.Models.Cesta;
using Core.Entities;
using Core.Exceptions;

namespace Aplicacao.Validacoes
{
    public static class ValidacaoCesta
    {
        public static void ValidarQuantidadeAtivos(CestaRequest request)
        {
            if (request.Itens.Count != 5)
                throw new CestaException(
                    $"A cesta deve conter exatamente 5 ativos. Quantidade informada: {request.Itens.Count}.",
                    "QUANTIDADE_ATIVOS_INVALIDA"
                );
        }

        public static void ValidarSomaPercentuais(CestaRequest request)
        {
            var percentualTotal = request.Itens.Sum(i => i.Percentual);
            if (percentualTotal < 100)
                throw new CestaException(
                    $"Soma dos percentuais diferente de 100%",
                    "PERCENTUAIS_INVALIDOS"
                );
        }

        public static void ValidarCestaAtiva(Cesta? cesta)
        {
            if (cesta is null)
                throw new CestaException("Não existe cesta ativa.", "CESTA_NAO_ENCONTRADA");
        }
    }
}
