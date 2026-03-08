using Aplicacao.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MotorRebalanceamento
{
    public class Worker : BackgroundService
    {
        private readonly IMotorRebalanceamentoService _motorRebalanceamento;

        public Worker(IMotorRebalanceamentoService motorRebalanceamento)
        {
            _motorRebalanceamento = motorRebalanceamento;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

        }
    }
}