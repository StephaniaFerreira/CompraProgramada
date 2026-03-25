using Core.Entities;
using Aplicacao.Interfaces;
using Core.Interfaces.MotorCompra;
using Aplicacao.Validacoes;

namespace Core.MotorCompra
{
    public class MotorCompraService : IMotorCompraService
    {
        private readonly IImpostoService _impostoService;
        private readonly IMotorCompraDomainService _motorCompraDomainService;
        private readonly IMotorCompraRepository _motorCompraCompraRepository;

        public MotorCompraService(IMotorCompraDomainService motorCompraDomainService, IMotorCompraRepository motorCompraCompraRepository, IImpostoService impostoService)
        {
            _impostoService = impostoService;
            _motorCompraDomainService = motorCompraDomainService;
            _motorCompraCompraRepository = motorCompraCompraRepository;
        }
        public int ExecutarMotorDeCompra(DateTime data)
        {
            ValidacaoCompra.EhDataDeExecucaoValida(data);

            var clientes = _motorCompraCompraRepository.ObterClientesAtivos();
            decimal valorAporteTotalMaster = clientes.Sum(c => c.ValorMensal / 3);

            var cestaVigente = _motorCompraCompraRepository.ObterCestaVigente();

            Dictionary<string, decimal> cotacoes = _motorCompraCompraRepository.ObterCotacaoPorTicker(cestaVigente.Itens, data);

            ValidacaoCompra.ExisteCotacao(cotacoes);

            var contaMaster = _motorCompraCompraRepository.ObterContaMaster();


            Dictionary<string, int> quantidadeAcaoAComprarPorTicker = new();

            foreach (var Ticker in cestaVigente!.Itens)
            {
               
                var valorTotalAtivo =_motorCompraDomainService.CalcularValorTotalAtivo(Ticker, valorAporteTotalMaster);
                
                var quantidadeAtivo = _motorCompraDomainService.CalcularQuantidadeAtivo(valorTotalAtivo, cotacoes, Ticker);

                var quantidadeRemanecente = _motorCompraCompraRepository.ObterQuantidadeRemanecenteCustodia(contaMaster, Ticker);

                quantidadeAcaoAComprarPorTicker[Ticker.Ticker] = _motorCompraDomainService.CalcularQuantidadeAtivoAComprar(quantidadeAtivo, quantidadeRemanecente);
                                              
                int quantidadeLotesPadrao = _motorCompraDomainService.CalcularQuantidadeLotesPadrao(quantidadeAcaoAComprarPorTicker, Ticker);
                
                int quantidadeFracionaria = _motorCompraDomainService.CalcularQuantidadeLotesFracionario(quantidadeAcaoAComprarPorTicker, Ticker);

                var ordens = _motorCompraDomainService.CriarOrdemCompraMaster(quantidadeLotesPadrao, quantidadeFracionaria, cotacoes, Ticker, data);

                _motorCompraCompraRepository.AdicionarOrdensMaster(ordens);

                CustodiaFilhote contaCustodiaFilhote = new();
                int totalDistribuido = 0;
                var novoPrecoMedio = 0m;

                foreach (var cliente in clientes)
                {
                    var valorAporteIndividual = _motorCompraDomainService.CalcularValorAporteIndividual(cliente);
                    var porcentagemAporteCliente = _motorCompraDomainService.CalcularPorcentagemAporteIndividual(valorAporteIndividual, valorAporteTotalMaster);

                    int quantidadeNova = _motorCompraDomainService.CalcularQuantidadeNovaAtivo(quantidadeAcaoAComprarPorTicker, Ticker, porcentagemAporteCliente);
                    totalDistribuido += quantidadeNova;

                    var custodiaAnterior = _motorCompraCompraRepository.ObterCustodiaFilhote(Ticker, cliente);

                    novoPrecoMedio = _motorCompraDomainService.CalcularNovoPrecoMedio(custodiaAnterior, quantidadeNova, cotacoes, Ticker);

                    var contaGraficaId = _motorCompraCompraRepository.ObterContaGraficaId(Ticker, cliente);

                    var custodia = _motorCompraDomainService.CriarCustodiaFilhote(custodiaAnterior, contaGraficaId, Ticker, quantidadeNova, novoPrecoMedio, cotacoes, data, valorAporteIndividual);

                    _motorCompraCompraRepository.AdicionarCustodiaFilhote(custodia);

                }

                var residuoAnterior = _motorCompraCompraRepository.ObterResiduoMaster(contaMaster, Ticker);

                var quantidadeResiduoAtual = _motorCompraDomainService.CalcularQuantidadeResiduo(quantidadeAcaoAComprarPorTicker[Ticker.Ticker], totalDistribuido);

                if (quantidadeResiduoAtual != 0)
                {
                    var residuo = _motorCompraDomainService.CriarOuAlterarResiduos(Ticker, quantidadeResiduoAtual, contaMaster, data, residuoAnterior, novoPrecoMedio, cotacoes[Ticker.Ticker]);

                    _motorCompraCompraRepository.AdicionarResiduos(residuo);
                }
                
            }

            _motorCompraCompraRepository.Salvar();

            return _impostoService.CalcularIRDedoDuro(data).GetAwaiter().GetResult();

        }
        

    }
}
