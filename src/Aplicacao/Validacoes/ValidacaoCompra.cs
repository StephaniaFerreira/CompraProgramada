using Core.Expections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacao.Validacoes
{
    public  class ValidacaoCompra
    {
        public static void ExisteCotacao(Dictionary<string, decimal> cotacoes)
        {
            if (cotacoes?.Count == 0)
                throw new RegraNegocioException(
                    $"Insera as cotacoes.",
                    "NAO_HA_COTACAO"
                );
        }
    }
}
