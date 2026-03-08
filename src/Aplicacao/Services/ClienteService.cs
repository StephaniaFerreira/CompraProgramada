using Aplicacao.Interfaces;
using Aplicacao.Models.Cliente.Adesao;
using Aplicacao.Models.Cliente.AlterarValorMensal;
using Aplicacao.Models.Cliente.Carteira;
using Aplicacao.Validacoes;
using Cliente.Models.Saida;
using Core.Interfaces;
using Core.Interfaces.Cliente;
using Microsoft.EntityFrameworkCore;

namespace Aplicacao.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteDomainService _clienteDomainService;
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteDomainService clienteDomainService, IClienteRepository clienteRepository)
        {
            _clienteDomainService = clienteDomainService;
            _clienteRepository = clienteRepository;
        }

        public AdesaoResponse Aderir(AdesaoRequest request)
        {
            ValidacaoCliente.ValidarAdesao(request);

            bool cpfExiste = _clienteRepository.ExisteCpf(request.Cpf!);

            ValidacaoCliente.ValidarExisteCPF(cpfExiste);

            var agora = DateTime.UtcNow;

            var cliente = _clienteDomainService.criarUsuario(request.Nome!, request.Cpf!, request.Email!, request.ValorMensal, agora);

            _clienteRepository.AdicionarCliente(cliente);
            _clienteRepository.Salvar();

            var conta = _clienteDomainService.CriarContaGrafica(cliente, agora);

            _clienteRepository.AdicionarContaGrafica(conta);
            _clienteRepository.Salvar();
            //verificar como salvar os dois juntos
            

            return new AdesaoResponse(
                cliente.Id,
                cliente.Nome,
                cliente.Cpf,
                cliente.Email,
                cliente.ValorMensal,
                cliente.Ativo,
                cliente.DataAdesao,
                new ContaGraficaResponse(
                    conta.Id,
                    conta.NumeroConta!,
                    conta.Tipo!,
                    conta.DataCriacao
                )
            );
        }

        public SaidaResponse Sair(int clienteId)
        {
            var cliente = _clienteRepository.ObterCliente(clienteId);

            ValidacaoCliente.ValidarExisteCliente(cliente);

            _clienteDomainService.DesativarCliente(cliente);

            _clienteRepository.Salvar();


            return new SaidaResponse(
                cliente.Id,
                cliente.Nome,
                cliente.Ativo,
                cliente.DataSaida!.Value,
                "Adesao encerrada. Sua posicao em custodia foi mantida."
            );
        }

        public AlterarValorMensalResponse AlterarValorMensal(int clienteId, AlterarValorMensalRequest request)
        {
            ValidacaoCliente.ValidarAlteracaoValor(request.NovoValorMensal);

            var cliente = _clienteRepository.ObterCliente(clienteId);

            ValidacaoCliente.ValidarExisteCliente(cliente);

            var valorAnterior = cliente!.ValorMensal;

            _clienteDomainService.AlterarValorMensal(cliente, request.NovoValorMensal);

            _clienteRepository.Salvar();


            return new AlterarValorMensalResponse(
                cliente.Id,
                valorAnterior,
                cliente.ValorMensal,
                DateTime.UtcNow,
                "Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra."
            );
        }

        public CarteiraResponse ConsultarCarteira(int clienteId)
        {
            var cliente = _clienteRepository.ObterCliente(clienteId);

            ValidacaoCliente.ValidarExisteCliente(cliente);

            var custodiasFilhotes = _clienteRepository.ObterCustodiasFilhotes(cliente);

            ValidacaoCliente.ValidarCustodiasFilhotes(custodiasFilhotes);

            decimal totalInvestido = _clienteDomainService.CalcularTotalInvestido(custodiasFilhotes!);
            decimal valorAtualTotal = _clienteDomainService.CalcularValorAtualTotal(custodiasFilhotes!);

            var listaAtivos = custodiasFilhotes.Select(item =>
            {
                decimal valorInvestidoNoAtivo = item!.Quantidade * item.PrecoMedio;
                decimal valorAtualDoAtivo = item.Quantidade * item.ValorAtual;

                return new AtivoCarteira(
                    item.Ticker,
                    item.Quantidade,
                    item.PrecoMedio,
                    item.ValorAtual,
                    valorAtualDoAtivo,
                    valorAtualDoAtivo - valorInvestidoNoAtivo,
                    valorInvestidoNoAtivo > 0 ? ((valorAtualDoAtivo / valorInvestidoNoAtivo) - 1) * 100 : 0,
                    valorAtualTotal > 0 ? (valorAtualDoAtivo / valorAtualTotal) * 100 : 0
                );
            }).ToList();

            var resumo = new ResumoCarteira(
                totalInvestido,
                valorAtualTotal,
                valorAtualTotal - totalInvestido,
                totalInvestido > 0 ? ((valorAtualTotal / totalInvestido) - 1) * 100 : 0
            );

            return new CarteiraResponse(
                cliente!.Id,
                cliente.Nome,
                cliente.ContaGrafica.NumeroConta ?? "N/A",
                DateTime.UtcNow,
                resumo,
                listaAtivos
            );
        }

        public RentabilidadeDetalhadaResponse ObterRentabilidadeDetalhada(int clienteId)
        {
            var cliente = _clienteRepository.ObterCliente(clienteId);

            ValidacaoCliente.ValidarExisteCliente(cliente);

            var custodiasFilhotes = _clienteRepository.ObterCustodiasFilhotes(cliente!);

            var distribuicoes = _clienteRepository.ObterDistribuicoesCliente(cliente.Id);

            decimal valorTotalInvestido = _clienteDomainService.CalcularTotalInvestido(custodiasFilhotes!);
            decimal valorAtualCarteira = _clienteDomainService.CalcularValorAtualTotal(custodiasFilhotes!);


            var resumo = new RentabilidadeResumo(
                valorTotalInvestido,
                valorAtualCarteira,
                valorAtualCarteira - valorTotalInvestido,
                valorTotalInvestido > 0 ? ((valorAtualCarteira / valorTotalInvestido) - 1) * 100 : 0
            );


            var historicoAportes = new List<HistoricoAporte>();

            var evolucaoCarteira = new List<EvolucaoCarteira>();

            decimal valorInvestidoAcumulado = 0;

            foreach (var d in distribuicoes)
            {
                valorInvestidoAcumulado += d.ValorAporte;

                historicoAportes.Add(new HistoricoAporte(
                    d.DataCriacao.ToString("yyyy-MM-dd"),
                    d.ValorAporte,
                    "1/1" 
                ));

                decimal valorMercadoNaData = 0;

                foreach (var ativo in d.Ativos)
                {

                    var cotacaoHistorica = _clienteRepository.ObterCotacao(ativo, d.DataCriacao);


                    valorMercadoNaData += ativo.Quantidade * cotacaoHistorica;
                }

                evolucaoCarteira.Add(new EvolucaoCarteira(
                                        d.DataCriacao.ToString("yyyy-MM-dd"),
                                        valorMercadoNaData,
                                        valorInvestidoAcumulado,
                                        valorInvestidoAcumulado > 0 ? ((valorMercadoNaData / valorInvestidoAcumulado) - 1) * 100 : 0
                 ));

            }

            return new RentabilidadeDetalhadaResponse(
                cliente!.Id,
                cliente.Nome,
                DateTime.UtcNow,
                resumo,
                historicoAportes,
                evolucaoCarteira
                );
        }
    }
}