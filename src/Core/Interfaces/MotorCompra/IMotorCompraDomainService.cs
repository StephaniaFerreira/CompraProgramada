using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.MotorCompra
{
    public interface IMotorCompraDomainService
    {
        decimal CalcularValorTotalAtivo(ItemCesta item, decimal valorAporteTotal);
        int CalcularQuantidadeAtivo(decimal valorTotalAtivo, Dictionary<string, decimal> cotacoes, ItemCesta item);
        int CalcularQuantidadeAtivoAComprar(int quantidaTotal, int quantidadeRemanecente);
        int CalcularQuantidadeLotesPadrao(Dictionary<string, int> quantidadeAcaoAComprarPorTicker, ItemCesta Ticker);
        int CalcularQuantidadeLotesFracionario(Dictionary<string, int> quantidadeAcaoAComprarPorTicker, ItemCesta Ticker);
        decimal CalcularValorAporteIndividual(ClienteCadastro cliente);
        int CalcularQuantidadeNovaAtivo(Dictionary<string, int> quantidadeAcaoAComprarPorTicker, ItemCesta Ticker, decimal porcentagemAporteCliente);
        decimal CalcularNovoPrecoMedio(CustodiaFilhote custodiaAnterior, int quantidadeNova, Dictionary<string, decimal> cotacoes, ItemCesta item);
        decimal CalcularPorcentagemAporteIndividual(decimal valorAporteIndividual, decimal valorAporteTotal);
        CustodiaFilhote CriarOuAlterarCustodiaFilhote(CustodiaFilhote custodiaAnterior, int contaGraficaId, ItemCesta Ticker, int quantidadeNova, decimal novoPrecoMedio, Dictionary<string, decimal> cotacoes, DateTime data);
        CustodiaMaster CriarOuAlterarResiduos(ItemCesta Ticker, int quantidadeAtivo, ContaMaster contaMaster, DateTime data, CustodiaMaster? residuoAnterior, decimal precoMedio, decimal valortual);
        List<Ordem> CriarOrdemCompraMaster(int quantidadeLotesPadrao, int quantidadeFracionaria, Dictionary<string, decimal> cotacoes, ItemCesta Ticker, DateTime data);
        List<CustodiaMaster> CriarCustodiaMaster(List<Ordem> ordens, ContaMaster contaMaster);
        int CalcularQuantidadeResiduo(int quantidadeAcaoAComprarPorTicker, int totalDistribuido);
    }
}
