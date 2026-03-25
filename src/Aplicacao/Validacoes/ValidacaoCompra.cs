using Core.Expections;
using BrazilHolidays.Net;

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
        public static DateTime ObterProximoDiaUtil(DateTime data)
        {
            while (EhFimDeSemana(data) || EhFeriado(data))
            {
                data = data.AddDays(1);
            }
            return data;
        }
        private static bool EhFimDeSemana(DateTime data)
        {
            return data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday;
        }
        private static bool EhFeriado(DateTime data)
        {
            return data.IsHoliday();
        }

        public static void EhDataDeExecucaoValida(DateTime data)
        {
            int[] diasAlvo = { 5, 15, 25 };

            foreach (var dia in diasAlvo)
            {
                DateTime dataTeorica = new DateTime(data.Year, data.Month, dia);

                DateTime dataExecucaoEsperada = ObterProximoDiaUtil(dataTeorica);

                if (data.Date == dataExecucaoEsperada.Date)
                    return;
            }

            throw new InvalidOperationException(
                $"A data {data:dd/MM/yyyy} não é um dia de execução válido (5, 15 ou 25 ou próximo dia útil).");
        }
    }
}
