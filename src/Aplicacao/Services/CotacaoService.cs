using Aplicacao.Interfaces;
using Core.Entities;
using Core.Interfaces.Cotacoes;

namespace Aplicacao.Services
{
    public class CotacaoService: ICotacaoService
    {
        private readonly ICotacaoRepository _cotacaoRepository;
        private readonly ICotacaoArquivoReader _reader;

        public CotacaoService(ICotacaoRepository cotacaoRepository, ICotacaoArquivoReader reader)
        {
            _cotacaoRepository = cotacaoRepository ?? throw new ArgumentNullException(nameof(cotacaoRepository));
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }
        public async Task ExecutarRegistroArquivo(DateTime data)
        {
            List<Cotacao> cotacoes = await _reader.LerAsync(data);

            var ticketsVigente = _cotacaoRepository.ObterTicketsCestaAtual();

            var cotacoesVigentes = cotacoes
                                    .Where(c => ticketsVigente.Contains(c.Ticker))
                                    .ToList();

            if (!_cotacaoRepository.ExisteRegistroParaData(data))
                _cotacaoRepository.PopularTabela(cotacoesVigentes);

        }

        
        
        
    }
}
