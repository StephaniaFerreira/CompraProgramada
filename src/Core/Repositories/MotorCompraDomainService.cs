using BrazilHolidays.Net;
using Core.Entities;
using Core.Interfaces.MotorCompra;

namespace Core.Repositories
{
    public class MotorCompraDomainService : IMotorCompraDomainService
    {

        public decimal CalcularValorTotalAtivo(ItemCesta Ticker, decimal valorAporteTotal)
        {
            return (Ticker.Percentual/100) * valorAporteTotal;
        }
        public int CalcularQuantidadeAtivo(decimal valorTotalAtivo, Dictionary<string, decimal> cotacoes, ItemCesta Ticker)
        {
            return (int)(Math.Truncate(valorTotalAtivo / cotacoes[Ticker.Ticker]));
        }
        public int CalcularQuantidadeAtivoAComprar(int quantidadeTotal, int quantidadeRemanecente)
        {
            return quantidadeTotal - quantidadeRemanecente;
        }
        public int CalcularQuantidadeLotesPadrao(Dictionary<string, int> quantidadeAcaoAComprarPorTicker, ItemCesta Ticker)
        {
            return quantidadeAcaoAComprarPorTicker[Ticker.Ticker] / 100;
        }
        public int CalcularQuantidadeLotesFracionario(Dictionary<string, int> quantidadeAcaoAComprarPorTicker, ItemCesta Ticker)
        {
            return quantidadeAcaoAComprarPorTicker[Ticker.Ticker] % 100;
        }
        public decimal CalcularValorAporteIndividual(ClienteCadastro cliente)
        {
            return cliente.ValorMensal / 3;
        }
        public decimal CalcularPorcentagemAporteIndividual(decimal valorAporteIndividual, decimal valorAporteTotal)
        {
            return valorAporteIndividual / valorAporteTotal;
        }
        public int CalcularQuantidadeNovaAtivo(Dictionary<string, int> quantidadeAcaoAComprarPorTicker, ItemCesta Ticker, decimal porcentagemAporteCliente)
        {
            return (int)Math.Truncate(quantidadeAcaoAComprarPorTicker[Ticker.Ticker] * porcentagemAporteCliente);
        }
        
        public List<Ordem> CriarOrdemCompraMaster(int quantidadeLotesPadrao, int quantidadeFracionaria, Dictionary<string, decimal> cotacoes, ItemCesta Ticker, DateTime data)
        {
            List<Ordem> Ordens = new();
            if (quantidadeLotesPadrao > 0)
            {
                Ordem ordemPadrao = new();
                ordemPadrao.Ticker = $"{Ticker.Ticker}";
                ordemPadrao.QuantidadeTotal = quantidadeLotesPadrao;
                ordemPadrao.ValorTotal = quantidadeLotesPadrao * cotacoes[Ticker.Ticker];
                ordemPadrao.PrecoUnitario = cotacoes[Ticker.Ticker];
                ordemPadrao.Detalhes.Add(new DetalheOrdem { Ticker = $"{Ticker.Ticker}", Quantidade = quantidadeLotesPadrao, Tipo = "PADRAO" });
                ordemPadrao.TipoOrdem = "Compra";
                ordemPadrao.DataCompra = data.Date;

                Ordens.Add(ordemPadrao);
            }
            if (quantidadeFracionaria > 0)
            {
                Ordem ordemFracionaria = new();
                ordemFracionaria.Ticker = $"{Ticker.Ticker}";
                ordemFracionaria.QuantidadeTotal = quantidadeFracionaria;
                ordemFracionaria.PrecoUnitario = cotacoes[Ticker.Ticker];
                ordemFracionaria.ValorTotal = ordemFracionaria.QuantidadeTotal * ordemFracionaria.PrecoUnitario;
                ordemFracionaria.Detalhes.Add(new DetalheOrdem { Ticker = $"{Ticker.Ticker}F", Quantidade = quantidadeFracionaria, Tipo = "FRACIONARIO" });
                ordemFracionaria.TipoOrdem = "Compra";
                ordemFracionaria.DataCompra = data.Date;

                Ordens.Add(ordemFracionaria);

            }
            return Ordens;
        }
        
        public List<CustodiaMaster> CriarCustodiaMaster(List<Ordem> ordens, ContaMaster contaMaster)
        {
            List<CustodiaMaster> custodias = new();
            foreach (var ordem in ordens)
            {
                var custodia = new CustodiaMaster
                {
                    Quantidade = ordem.QuantidadeTotal,
                    Ticker = ordem.Ticker,
                    PrecoMedio = 0,
                    ValorAtual = ordem.PrecoUnitario,
                    Origem = $"Compra {ordem.DataCompra}",
                    ContaMasterId = contaMaster.Id
                };
                custodias.Add(custodia);
            }
                

            return custodias;
        }
        public decimal CalcularNovoPrecoMedio(CustodiaFilhote custodiaAnterior, int quantidadeNova, Dictionary<string, decimal> cotacoes, ItemCesta Ticker)
        {

            decimal qtdAnterior = custodiaAnterior?.Quantidade ?? 0;
            decimal precoMedioAnterior = custodiaAnterior?.PrecoMedio ?? 0;

            decimal novoPrecoMedio = ((qtdAnterior * precoMedioAnterior) + (quantidadeNova * cotacoes[Ticker.Ticker])) / (qtdAnterior + quantidadeNova);
            return novoPrecoMedio;
        }
        public CustodiaFilhote CriarOuAlterarCustodiaFilhote(CustodiaFilhote custodiaAnterior, int contaGraficaId, ItemCesta Ticker, int quantidadeNova, decimal novoPrecoMedio, Dictionary<string, decimal> cotacoes, DateTime data)
        {
            var custodia = custodiaAnterior;

            if (custodiaAnterior is not null)
            {
                custodiaAnterior.Quantidade += quantidadeNova;
                custodiaAnterior.PrecoMedio = novoPrecoMedio;
                custodiaAnterior.ValorAtual = cotacoes[Ticker.Ticker];
                custodiaAnterior.DataUltimaAtualizacao = data.Date;
            }
            else
            {
                custodia = new CustodiaFilhote
                {
                    ContaGraficaId = contaGraficaId,
                    Ticker = Ticker.Ticker,
                    Quantidade = quantidadeNova,
                    PrecoMedio = novoPrecoMedio,
                    ValorAtual = cotacoes[Ticker.Ticker],
                    DataUltimaAtualizacao = data.Date
                };
            }

            return custodia;
        }
        public CustodiaMaster CriarOuAlterarResiduos(ItemCesta Ticker, int quantidadeResiduoAtual, ContaMaster contaMaster, DateTime data, CustodiaMaster? residuoAnterior, decimal precoMedio, decimal valorAtual)
        {
            var residuo = residuoAnterior;

            if (residuo is not null)
                residuo.Quantidade = quantidadeResiduoAtual;
            else
            {
                residuo = new CustodiaMaster {
                    Quantidade = quantidadeResiduoAtual,
                    Ticker = Ticker.Ticker,
                    PrecoMedio = precoMedio,
                    ValorAtual = valorAtual,
                    Origem = $"Residuo distribuicao {data.Date}",
                    ContaMasterId = contaMaster.Id 
                };
            }
                
            return residuo;

        }
        public int CalcularQuantidadeResiduo(int quantidadeAcaoAComprarPorTicker, int totalDistribuido)
        {
            return quantidadeAcaoAComprarPorTicker - totalDistribuido;
        }


    }
}
