using Aplicacao.Interfaces;
using Aplicacao.Models.Cesta;
using Aplicacao.Validacoes;
using Core.Entities;
using Core.Interfaces.Backoffice;


namespace Aplicacao.Services
{
    public class BackofficeService : IBackofficeService
    {
        private readonly IBackofficeDomainService _backofficeDomainService;
        private readonly IBackofficeRepository _backofficeRepository;
        private readonly IMotorCompraService _motorCompra;

        public BackofficeService(IBackofficeDomainService backofficeDomainService, IBackofficeRepository backofficeRepository, IMotorCompraService motorCompra)
        {
            _backofficeDomainService = backofficeDomainService ?? throw new ArgumentNullException(nameof(backofficeDomainService));
            _backofficeRepository = backofficeRepository ?? throw new ArgumentNullException(nameof(backofficeRepository));
            _motorCompra = motorCompra ?? throw new ArgumentNullException(nameof(motorCompra));
        }

        public CestaResponse CadastrarOuAlterar(CestaRequest request)
        {

            ValidacaoCesta.ValidarQuantidadeAtivos(request);
            ValidacaoCesta.ValidarSomaPercentuais(request);

            var agora = DateTime.UtcNow;
            bool primeraCesta = false;

            var cestaAtual = _backofficeRepository.ObterCestaAtual();

            if (cestaAtual is null)
                primeraCesta = true;

            bool rebalanceamento = false;
            CestaAnteriorResponse? cestaAnterior = null;
            List<string>? ativosRemovidos = null;
            List<string>? ativosAdicionados = null;

            if (!primeraCesta)
            {
                _backofficeDomainService.DesativarCesta(cestaAtual!, agora);
                

                cestaAnterior = new CestaAnteriorResponse(
                    cestaAtual!.Id,
                    cestaAtual.Nome,
                    cestaAtual.DataDesativacao!.Value
                );

                ativosRemovidos = cestaAtual.Itens
                    .Where(i => !request.Itens.Any(r => r.Ticker == i.Ticker))
                    .Select(i => i.Ticker)
                    .ToList();

                ativosAdicionados = request.Itens
                    .Where(i => !cestaAtual.Itens.Any(r => r.Ticker == i.Ticker))
                    .Select(i => i.Ticker)
                    .ToList();

                rebalanceamento = ativosRemovidos.Any() || ativosAdicionados.Any();
            }

            var novaCesta = new Cesta
            {
                Nome = request.Nome,
                Ativa = true,
                DataCriacao = agora,
                Itens = request.Itens
                    .Select(i => new ItemCesta { Ticker = i.Ticker, Percentual = i.Percentual })
                    .ToList()
            };

            _backofficeRepository.AdicionarCesta(novaCesta);
            _backofficeRepository.Salvar();

            var clientesAtivos = _backofficeRepository.ObterTotalClientesAtivos();

            //disparar rebalanceamento

            return new CestaResponse(
                novaCesta.Id,
                novaCesta.Nome,
                novaCesta.Ativa,
                novaCesta.DataCriacao,
                novaCesta.Itens.Select(i => new ItemResponse(i.Ticker, i.Percentual)).ToList(),
                rebalanceamento,
                rebalanceamento
                    ? $"Cesta atualizada. Rebalanceamento disparado para {clientesAtivos} clientes ativos."
                    : "Primeira cesta cadastrada com sucesso.",
                null,
                cestaAnterior,
                ativosRemovidos,
                ativosAdicionados
            );
        }

        public CestaAtualResponse ConsultarAtual()
        {
            var cesta = _backofficeRepository.ObterCestaAtual();

            ValidacaoCesta.ValidarCestaAtiva(cesta);

            var cotacoesPorTicker = _backofficeRepository.ObterCotacaoPorTicker(cesta!.Itens!);

            var itensComCotacao = cesta!.Itens
                .Select(i => new ItemAtualResponse(
                    i.Ticker,
                    i.Percentual,
                    i.CotacaoAtual ?? cotacoesPorTicker[i.Ticker]
                ))
                .ToList();

            return new CestaAtualResponse(
                cesta.Id,
                cesta.Nome,
                cesta.Ativa,
                cesta.DataCriacao,
                itensComCotacao
                );
         }

        public HistoricoCestaResponse Historico()
        {
            var cestas = _backofficeRepository.ObterCestas();

            var response = cestas.Select(c => new CestaResponse(
                c.Id,
                c.Nome,
                c.Ativa,
                c.DataCriacao,
                c.Itens.Select(i => new ItemResponse(i.Ticker, i.Percentual)).ToList(),
                RebalanceamentoDisparado: false,
                Mensagem: null,
                c.DataDesativacao,
                CestaAnteriorDesativada: null
            )).ToList();

            return new HistoricoCestaResponse(response);
        }

        public CustodiaMasterResponse ConsultarCustodiaMaster()
        {
            var contaMaster = _backofficeRepository.ObterContaMaster();

            var itensCustodia = _backofficeRepository.ObterCustodiaMaster();

            var response = new CustodiaMasterResponse
            {
                ContaMaster = new ContaMaster
                {
                    Id = contaMaster!.Id,
                    NumeroConta = contaMaster!.NumeroConta!,
                    Tipo = contaMaster.Tipo!
                },
                Custodia = itensCustodia
            };

            response.ValorTotalResiduo = response.Custodia.Sum(x => x.ValorAtual * x.Quantidade);

            return response;
        }

        public ExecucaoCompraResponse ExecutarCompraMotor(ExecucaoCompraRequest request)
        {

            var quantidadeMsg = _motorCompra.ExecutarMotorDeCompra(request.DataReferencia);

            var ordens = _backofficeRepository.ObterOrdens(request.DataReferencia);
            var custodiasFilhotes = _backofficeRepository.ObterCustodiaFilhotes(request.DataReferencia);
            var residuosCustMaster = _backofficeRepository.ObterCustodiaMaster();
            var clientes = _backofficeRepository.ObterClientesAtivos();

            var distribuicoes = new List<DistribuicaoClienteResponse>();

            foreach (var cliente in clientes)
            {
                var custodiasCliente = custodiasFilhotes
                    .Where(c => c.ContaGraficaId == cliente.ContaGrafica.Id)
                    .ToList();

                var ativosDistribuidos = custodiasCliente
                    .Select(c => new AtivoDistribuidoResponse
                    {
                        Ticker = c.Ticker,
                        Quantidade = c.Quantidade
                    })
                    .ToList();

                var distribuicao = new DistribuicaoClienteResponse
                {
                    ClienteId = cliente.Id,
                    Nome = cliente.Nome,
                    ValorAporte = custodiasCliente.Sum(c => c.ValorAtual),
                    Ativos = ativosDistribuidos
                };

                distribuicoes.Add(distribuicao);
            }

            var residuos = new List<ResiduoMasterResponse>();

            foreach (var residuo in residuosCustMaster)
            {
                var r = new ResiduoMasterResponse
                {
                    Ticker = residuo.Ticker,
                    Quantidade = residuo.Quantidade,
                };

                residuos.Add(r);
            }

            var ordensResponse = ordens.Select(o => new OrdemResponse
            {
                Ticker = o.Ticker,
                QuantidadeTotal = o.QuantidadeTotal,
                Detalhes = o.Detalhes.Select(d => new DetalheOrdemResponse
                {
                    Tipo = d.Tipo,
                    Ticker = d.Ticker,
                    Quantidade = d.Quantidade
                }).ToList(),
                ValorTotal = o.ValorTotal,
                PrecoUnitario = o.PrecoUnitario
            }).ToList();

            var response = new ExecucaoCompraResponse {
                    DataExecucao = request.DataReferencia,
                    TotalClientes = clientes.Count,
                    TotalConsolidado = ordens.Sum(o => o.ValorTotal),
                    OrdensCompra = ordensResponse,
                    Distribuicoes = distribuicoes,
                    ResiduosCustMaster = residuos,
                    EventosIRPublicados = quantidadeMsg,
                    Mensagem = $"Compra programada executada com sucesso para {clientes.Count} clientes"
            };

                return response;
        }
    }
}
