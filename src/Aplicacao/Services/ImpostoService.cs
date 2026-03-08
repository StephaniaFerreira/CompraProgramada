using Aplicacao.Models.Evento;
using Core.Interfaces;
using Infraestrutura.Kafka;

namespace Aplicacao.Interfaces
{
    public class ImpostoService : IImpostoService
    {
        private readonly IDbContext _context;
        private readonly IKafkaProducer _kafka;

        public ImpostoService(IDbContext context, IKafkaProducer kafka)
        {
            _context = context;
            _kafka = kafka;
        }

        public async Task CalcularIRDedoDuro(DateTime dataReferencia)
        {
            var clientes = _context.Clientes
                .Where(c => c.Ativo)
                .ToList();

            foreach (var cliente in clientes)
            {
                var contaGraficaId = _context.ContasGraficas.Where(c => c.ClienteId == cliente.Id).Select(c => c.Id).First();
                var custodias = _context.CustodiasFilhotes
                    .Where(c => c.ContaGraficaId == contaGraficaId)
                    .ToList();


                foreach (var ativo in custodias)
                {
                    var cotacaoAtual = _context.Cotacoes
                        .Where(c => c.Ticker == ativo.Ticker)
                        .OrderByDescending(c => c.DataPregao)
                        .Select(c => c.PrecoFechamento)
                        .First();

                    var valorTotal = cotacaoAtual * ativo.Quantidade;

                    var imposto = CalcularAliquota(valorTotal);

                    await RegistrarImposto(cliente.Id, cliente.Cpf, valorTotal, imposto, dataReferencia);

                }

            }
        }

        private static decimal CalcularAliquota(decimal valorTotal)
        {
            if (valorTotal <= 0)
                return 0;

            return valorTotal * 0.00005m; // 0,005%
        }

        private async Task RegistrarImposto(int clienteId,string CPF,decimal valorOperacao, decimal valorImposto, DateTime data)
        {
            var evento = new IRDedoDuroCalculado
            {
                ClienteId = clienteId,
                CPF = CPF,
                ValorOperacao = valorOperacao,
                ValorImposto = valorImposto,
                Data = data
            };

            try
            {
                await _kafka.PublicarAsync("imposto-calculado", evento);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Erro ao enviar para o Kafka: {ex.Message}");
            }
        }

        public void CalcularIRsobreVendas(DateTime data)
        {
            var clientes = _context.Clientes
                .Where(c => c.Ativo)
                .ToList();

            foreach (var cliente in clientes)
            {
                var contaGraficaId = _context.ContasGraficas.Where(c => c.ClienteId == cliente.Id).Select(c => c.Id).First();
                var custodias = _context.CustodiasFilhotes
                    .Where(c => c.ContaGraficaId == contaGraficaId && c.Quantidade == 0 && c.DataUltimaAtualizacao.Month == data.Month)
                    .ToList();

                var valorTotalVendas = custodias.Sum(c => c.PrecoMedio * c.Quantidade);
                var LucroLiquidoTotal = custodias.Sum(c => (c.ValorAtual * c.Quantidade) - (c.PrecoMedio * c.Quantidade));
                decimal valorImposto = 0;

                if (valorTotalVendas > 20000)
                    valorImposto = LucroLiquidoTotal * 0.2m;
                
            }
        }
    }
}
