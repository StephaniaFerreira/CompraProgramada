using Aplicacao.Interfaces;
using Core.Entities;
using Core.Interfaces;
using FileHelpers;
using Aplicacao.Models.Arquivo;

namespace Aplicacao.Services
{
    public class CotacaoService: ICotacaoService
    {
        private readonly IDbContext _context;
        public CotacaoService(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void ExecutarRegistroArquivo(DateTime data)
        {
            List<Cotacao> cotacoes = LerArquivo(data);

            if(!ExisteRegistroParaData(data))
                PopularTabela(cotacoes);
        }

        private static List<Cotacao> LerArquivo(DateTime data)
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
                    if (registro.TIPREG != "01")
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

            return cotacoes;
        }
        private void PopularTabela(List<Cotacao> cotacoes)
        {
            _context.Cotacoes.AddRange(cotacoes);
            _context.SaveChanges();

        }
        private bool ExisteRegistroParaData(DateTime data)
        {
            return _context.Cotacoes.Any(c => c.DataRegistro == data.Date);
        }
    }
}
