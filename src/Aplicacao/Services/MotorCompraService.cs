using BrazilHolidays.Net;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.Entities;
using Aplicacao.Interfaces;
using Core.Interfaces.MotorCompra;
using Aplicacao.Validacoes;

namespace Core.MotorCompra
{
    public class MotorCompraService : IMotorCompraService
    {
        private readonly IDbContext _context;
        private readonly IImpostoService _impostoService;
        private readonly IMotorCompraDomainService _motorCompraDomainService;
        private readonly IMotorCompraRepository _motorCompraCompraRepository;

        public MotorCompraService(IDbContext context, IMotorCompraDomainService motorCompraDomainService, IMotorCompraRepository motorCompraCompraRepository, IImpostoService impostoService)
        {
            _context = context;
            _impostoService = impostoService;
            _motorCompraDomainService = motorCompraDomainService;
            _motorCompraCompraRepository = motorCompraCompraRepository;
        }
        public void ExecutarMotorDeCompra(DateTime data)
        {
            if (!EhDataDeExecucaoValida(data))
            {
                throw new InvalidOperationException($"A data {data:dd/MM/yyyy} não é um dia de execução válido (5, 15 ou 25).");
            }


            var clientes = _motorCompraCompraRepository.ObterClientesAtivos();
            decimal valorAporteTotal = clientes.Sum(c => c.ValorMensal / 3);

            var cestaVigente = _motorCompraCompraRepository.ObterCestaVigente();

            Dictionary<string, decimal> cotacoes = _motorCompraCompraRepository.ObterCotacaoPorTicket(cestaVigente.Itens, data);

            ValidacaoCompra.ExisteCotacao(cotacoes);

            var contaMaster = _motorCompraCompraRepository.ObterContaMaster();


            Dictionary<string, int> quantidadeAcaoAComprarPorTicket = new();

            foreach (var ticket in cestaVigente!.Itens)
            {
               
                var valorTotalAtivo =_motorCompraDomainService.CalcularValorTotalAtivo(ticket, valorAporteTotal);
                
                var quantidadeAtivo = _motorCompraDomainService.CalcularQuantidadeAtivo(valorTotalAtivo, cotacoes, ticket);

                var quantidadeRemanecente = _motorCompraCompraRepository.ObterQuantidadeRemanecenteCustodia(contaMaster, ticket);

                quantidadeAcaoAComprarPorTicket[ticket.Ticker] = _motorCompraDomainService.CalcularQuantidadeAtivoAComprar(quantidadeAtivo, quantidadeRemanecente);
                                              
                int quantidadeLotesPadrao = _motorCompraDomainService.CalcularQuantidadeLotesPadrao(quantidadeAcaoAComprarPorTicket, ticket);
                
                int quantidadeFracionaria = _motorCompraDomainService.CalcularQuantidadeLotesFracionario(quantidadeAcaoAComprarPorTicket, ticket);

                var ordens = _motorCompraDomainService.CriarOrdemCompraMaster(quantidadeLotesPadrao, quantidadeFracionaria, cotacoes, ticket);

                _motorCompraCompraRepository.AdicionarOrdensMaster(ordens);

                var custodiasMaster = _motorCompraDomainService.CriarCustodiaMaster(ordens, contaMaster);

                _motorCompraCompraRepository.AdicionarCustodiaMaster(custodiasMaster);

                CustodiaFilhote contaCustodiaFilhote = new();
                int totalDistribuido = 0;

                foreach (var cliente in clientes)
                {
                    var valorAporteIndividual = _motorCompraDomainService.CalcularValorAporteIndividual(cliente);
                    var porcentagemAporteCliente = _motorCompraDomainService.CalcularPorcentagemAporteIndividual(valorAporteIndividual, valorAporteTotal);

                    int quantidadeNova = _motorCompraDomainService.CalcularQuantidadeNovaAtivo(quantidadeAcaoAComprarPorTicket, ticket, porcentagemAporteCliente);
                    totalDistribuido += quantidadeNova;

                    var custodiaAnterior = _motorCompraCompraRepository.ObterCustodiaFilhote(ticket, cliente);

                    var novoPrecoMedio = _motorCompraDomainService.CalcularNovoPrecoMedio(custodiaAnterior, quantidadeNova, cotacoes, ticket);

                    var contaGraficaId = _motorCompraCompraRepository.ObterContaGraficaId(ticket, cliente);

                    var custodia = _motorCompraDomainService.CriarOuAlterarCustodiaFilhote(custodiaAnterior, contaGraficaId, ticket, quantidadeNova, novoPrecoMedio, cotacoes);

                    _motorCompraCompraRepository.AdicionarCustodiaFilhote(custodia);

                }

                //var residuoAnterior = _motorCompraCompraRepository.ObterResiduoMaster(contaMaster, ticket);

                //var residuo = _motorCompraDomainService.AlterarResiduos(totalDistribuido, ticket, quantidadeAtivo, contaMaster, data, residuoAnterior!);

                //_motorCompraCompraRepository.AdicionarResiduos(residuo);
            }

            _motorCompraCompraRepository.Salvar();

            _impostoService.CalcularIRDedoDuro(data).GetAwaiter().GetResult();

        }
        public DateTime ObterProximoDiaUtil(DateTime data)
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

        private bool EhDataDeExecucaoValida(DateTime data)
        {
            int[] diasAlvo = { 5, 15, 25 };

            foreach (var dia in diasAlvo)
            {
                DateTime dataTeorica = new DateTime(data.Year, data.Month, dia);

                DateTime dataExecucaoEsperada = ObterProximoDiaUtil(dataTeorica);

                if (data.Date == dataExecucaoEsperada.Date)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
