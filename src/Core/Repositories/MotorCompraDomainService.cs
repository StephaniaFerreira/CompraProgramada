using BrazilHolidays.Net;
using Core.Entities;
using Core.Interfaces.MotorCompra;

namespace Core.Repositories
{
    public class MotorCompraDomainService : IMotorCompraDomainService
    {

        public decimal CalcularValorTotalAtivo(ItemCesta ticket, decimal valorAporteTotal)
        {
            return (ticket.Percentual/100) * valorAporteTotal;
        }
        public int CalcularQuantidadeAtivo(decimal valorTotalAtivo, Dictionary<string, decimal> cotacoes, ItemCesta ticket)
        {
            return (int)(Math.Truncate(valorTotalAtivo / cotacoes[ticket.Ticker]));
        }
        public int CalcularQuantidadeAtivoAComprar(int quantidadeTotal, int quantidadeRemanecente)
        {
            return quantidadeTotal - quantidadeRemanecente;
        }
        public int CalcularQuantidadeLotesPadrao(Dictionary<string, int> quantidadeAcaoAComprarPorTicket, ItemCesta ticket)
        {
            return quantidadeAcaoAComprarPorTicket[ticket.Ticker] / 100;
        }
        public int CalcularQuantidadeLotesFracionario(Dictionary<string, int> quantidadeAcaoAComprarPorTicket, ItemCesta ticket)
        {
            return quantidadeAcaoAComprarPorTicket[ticket.Ticker] % 100;
        }
        public decimal CalcularValorAporteIndividual(ClienteCadastro cliente)
        {
            return cliente.ValorMensal / 3;
        }
        public decimal CalcularPorcentagemAporteIndividual(decimal valorAporteIndividual, decimal valorAporteTotal)
        {
            return valorAporteIndividual / valorAporteTotal;
        }
        public int CalcularQuantidadeNovaAtivo(Dictionary<string, int> quantidadeAcaoAComprarPorTicket, ItemCesta ticket, decimal porcentagemAporteCliente)
        {
            return (int)Math.Truncate(quantidadeAcaoAComprarPorTicket[ticket.Ticker] * porcentagemAporteCliente);
        }
        
        public List<Ordem> CriarOrdemCompraMaster(int quantidadeLotesPadrao, int quantidadeFracionaria, Dictionary<string, decimal> cotacoes, ItemCesta ticket)
        {
            List<Ordem> Ordens = new();
            if (quantidadeLotesPadrao > 0)
            {
                Ordem ordemPadrao = new();
                ordemPadrao.Ticker = $"{ticket.Ticker}";
                ordemPadrao.QuantidadeTotal = quantidadeLotesPadrao;
                ordemPadrao.ValorTotal = quantidadeLotesPadrao * cotacoes[ticket.Ticker];
                ordemPadrao.PrecoUnitario = cotacoes[ticket.Ticker];
                ordemPadrao.Detalhes.Add(new DetalheOrdem { Ticker = $"{ticket.Ticker}", Quantidade = quantidadeLotesPadrao, Tipo = "PADRAO" });
                ordemPadrao.TipoOrdem = "Compra";
                ordemPadrao.DataCompra = DateTime.Now.Date;

                Ordens.Add(ordemPadrao);
            }
            if (quantidadeFracionaria > 0)
            {
                Ordem ordemFracionaria = new();
                ordemFracionaria.Ticker = $"{ticket.Ticker}";
                ordemFracionaria.QuantidadeTotal = quantidadeFracionaria;
                ordemFracionaria.PrecoUnitario = cotacoes[ticket.Ticker];
                ordemFracionaria.ValorTotal = ordemFracionaria.QuantidadeTotal * ordemFracionaria.PrecoUnitario;
                ordemFracionaria.Detalhes.Add(new DetalheOrdem { Ticker = $"{ticket.Ticker}F", Quantidade = quantidadeFracionaria, Tipo = "FRACIONARIO" });
                ordemFracionaria.TipoOrdem = "Compra";
                ordemFracionaria.DataCompra = DateTime.Now.Date;

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
        public decimal CalcularNovoPrecoMedio(CustodiaFilhote custodiaAnterior, int quantidadeNova, Dictionary<string, decimal> cotacoes, ItemCesta ticket)
        {

            decimal qtdAnterior = custodiaAnterior?.Quantidade ?? 0;
            decimal precoMedioAnterior = custodiaAnterior?.PrecoMedio ?? 0;

            decimal novoPrecoMedio = ((qtdAnterior * precoMedioAnterior) + (quantidadeNova * cotacoes[ticket.Ticker])) / (qtdAnterior + quantidadeNova);
            return novoPrecoMedio;
        }
        public CustodiaFilhote CriarOuAlterarCustodiaFilhote(CustodiaFilhote custodiaAnterior, int contaGraficaId, ItemCesta ticket, int quantidadeNova, decimal novoPrecoMedio, Dictionary<string, decimal> cotacoes)
        {
            var custodia = custodiaAnterior;

            if (custodiaAnterior is not null)
            {
                custodiaAnterior.Quantidade += quantidadeNova;
                custodiaAnterior.PrecoMedio = novoPrecoMedio;
                custodiaAnterior.ValorAtual = cotacoes[ticket.Ticker];
                custodiaAnterior.DataUltimaAtualizacao = DateTime.UtcNow;
            }
            else
            {
                custodia = new CustodiaFilhote
                {
                    ContaGraficaId = contaGraficaId,
                    Ticker = ticket.Ticker,
                    Quantidade = quantidadeNova,
                    PrecoMedio = novoPrecoMedio,
                    ValorAtual = cotacoes[ticket.Ticker],
                    DataUltimaAtualizacao = DateTime.UtcNow
                };
            }

            return custodia;
        }
        public CustodiaMaster AlterarResiduos(int totalDistribuido, ItemCesta ticket, int quantidadeAtivo, ContaMaster contaMaster, DateTime data, CustodiaMaster residuoAnterior)
        {
            int residuoMaster = 0;
            residuoMaster = quantidadeAtivo - totalDistribuido;

            var residuo = residuoAnterior;

            if (residuo is not null)
                residuo.Quantidade = residuoMaster;
            else
            {
                residuo = new CustodiaMaster {
                    Quantidade = residuoMaster,
                    Ticker = ticket.Ticker,
                    PrecoMedio = 0,
                    ValorAtual = 0,
                    Origem = $"Residuo distribuicao {data.Date}",
                    ContaMasterId = contaMaster.Id 
                };
            }
                
            return residuo;

        }
       
    }
}
