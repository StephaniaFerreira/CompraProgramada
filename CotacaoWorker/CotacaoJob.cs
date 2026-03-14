using Aplicacao.Interfaces;
using Quartz;

namespace CotacaoWorker
{
    public class CotacaoJob : IJob
    {
        private readonly ICotacaoService _cotacaoService;

        public CotacaoJob(ICotacaoService cotacaoService)
        {
            _cotacaoService = cotacaoService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _cotacaoService.ExecutarRegistroArquivo(DateTime.Now.Date);
        }
    }
}