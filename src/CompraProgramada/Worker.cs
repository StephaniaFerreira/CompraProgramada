using Aplicacao.Interfaces;

namespace CompraProgramada
{
    public class Worker : BackgroundService
    {
        private readonly IMotorCompraService _motorCompra;

        public Worker(IMotorCompraService motorCompra)
        {
            _motorCompra = motorCompra;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var hoje = DateTime.Today;

                
                int[] diasAlvo = { 5, 15, 25 };

                foreach (var dia in diasAlvo)
                {
                    
                    var dataBase = new DateTime(hoje.Year, hoje.Month, dia);
                    var dataExecucaoReal = _motorCompra.ObterProximoDiaUtil(dataBase);

                    
                    if (hoje.Date == dataExecucaoReal.Date)
                    {
                        _motorCompra.ExecutarMotorDeCompra(hoje.Date);
                        break; 
                    }
                }

                // Espera 24 horas para a próxima verificação
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }

        }

        
    }
}



