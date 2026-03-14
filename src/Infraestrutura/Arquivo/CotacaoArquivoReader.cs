using Aplicacao.Models.Arquivo;
using Core.Entities;
using Core.Interfaces.Cotacoes;
using FileHelpers;

namespace Infraestrutura.Arquivo
{
    public class CotacaoArquivoReader : ICotacaoArquivoReader
    {
        const string DETALHE = "01";
        public Task<List<Cotacao>> LerAsync(DateTime data)
        {
            var rootPath = Path.GetFullPath(
                                    Path.Combine(Directory.GetCurrentDirectory(), @"..\..")
                                    );

            var fileName = $"COTAHIST_D{data:ddMMyyyy}.txt";

            var path = Path.Combine(rootPath, "cotacoes", fileName);

            var engine = new FileHelperAsyncEngine<CotahistRegistro>();
            var cotacoes = new List<Cotacao>();


            using (engine.BeginReadFile(path))
            {
                CotahistRegistro registro;

                while ((registro = engine.ReadNext()) != null)
                {
                    if (registro.TIPREG != DETALHE)
                        continue;

                    Cotacao cotacao = new();
                    cotacao.Ticker = registro.CODNEG;
                    cotacao.DataPregao = registro.DATPRE;
                    cotacao.PrecoFechamento = registro.PREULT;
                    cotacao.PrecoAbertura = registro.PREABE;
                    cotacao.PrecoMaximo = registro.PREMAX;
                    cotacao.PrecoMinimo = registro.PREMIN;
                    cotacao.TipoMercado = int.Parse(registro.TPMERC);
                    cotacao.CodigoBDI = registro.CODBDI;
                    cotacao.DataRegistro = data.Date;

                    cotacoes.Add(cotacao);
                }
            }

            return Task.FromResult(cotacoes);
        }
    }
}
