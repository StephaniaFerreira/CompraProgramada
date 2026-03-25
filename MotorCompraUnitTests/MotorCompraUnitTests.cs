using Core.Entities;
using Core.Service;
using System;
using System.Collections.Generic;
using Xunit;

namespace MotorCompraUnitTests
{
    public class MotorCompraUnitTests
    {
        private readonly MotorCompraDomainService _service;

        public MotorCompraUnitTests()
        {
            _service = new MotorCompraDomainService();
        }

        [Fact]
        public void DeveCalcularNovoPrecoMedioCorretamente()
        {
            //Arrange

            var anterior = new CustodiaFilhote
            {
                Quantidade = 10,
                PrecoMedio = 20
            };

            var cotacoes = new Dictionary<string, decimal>
            {
                {"PETR4", 30}
            };

            //Act
            var resultado = _service.CalcularNovoPrecoMedio(
                anterior,
                10,
                cotacoes,
                new ItemCesta { Ticker = "PETR4" }
            );
            //Assert
            Assert.Equal(25, resultado);
        }

        [Fact]
        public void DeveCalcularValorTotalAtivoCorretamente()
        {
            //Arrange

            var itemCesta = new ItemCesta
            {
                Ticker = "PETR4",
                Percentual = 30
            };

            //Act
            var resultado = _service.CalcularValorTotalAtivo(itemCesta, 1000);
            //Assert
            Assert.Equal(300m, resultado);
        }

        [Fact]
        public void DeveCalcularQuantidadeAtivoCorretamente()
        {
            //Arrange

            var itemCesta = new ItemCesta
            {
                Ticker = "PETR4",
                Percentual = 30
            };

            var cotacoes = new Dictionary<string, decimal>
            {
                {"PETR4", 37}
            };

            //Act
            var resultado = _service.CalcularQuantidadeAtivo(1000, cotacoes, itemCesta);

            //Assert
            Assert.Equal(27, resultado);
        }

        [Fact]
        public void DeveCalcularQuantidadeAtivoAComprarCorretamente()
        {
            //Arrange

            var itemCesta = new ItemCesta
            {
                Ticker = "PETR4",
                Percentual = 30
            };

            var cotacoes = new Dictionary<string, decimal>
            {
                {"PETR4", 37}
            };

            //Act
            var resultado = _service.CalcularQuantidadeAtivoAComprar(27, 7);

            //Assert
            Assert.Equal(20, resultado);
        }

        [Fact]
        public void DeveCalcularQuantidadeLotesPadraoCorretamente()
        {
            //Arrange

            var itemCesta = new ItemCesta
            {
                Ticker = "PETR4",
                Percentual = 30
            };

            var quantidadeAcaoAComprarPorTicker = new Dictionary<string, int>
            {
                {"PETR4", 20}
            };

            //Act
            var resultado = _service.CalcularQuantidadeLotesPadrao(quantidadeAcaoAComprarPorTicker, itemCesta);

            //Assert
            Assert.Equal(0, resultado);
        }

        [Fact]
        public void DeveCalcularQuantidadeLotesFracionarioCorretamente()
        {
            //Arrange

            var itemCesta = new ItemCesta
            {
                Ticker = "PETR4",
                Percentual = 30
            };

            var quantidadeAcaoAComprarPorTicker = new Dictionary<string, int>
            {
                {"PETR4", 20}
            };

            //Act
            var resultado = _service.CalcularQuantidadeLotesFracionario(quantidadeAcaoAComprarPorTicker, itemCesta);

            //Assert
            Assert.Equal(20, resultado);
        }

        [Fact]
        public void DeveCalcularValorAporteIndividualCorretamente()
        {
            //Arrange

            var cliente = new ClienteCadastro
            {
                ValorMensal = 3000
            };

            //Act
            var resultado = _service.CalcularValorAporteIndividual(cliente);

            //Assert
            Assert.Equal(1000m, resultado);
        }

        [Fact]
        public void DeveCalcularPorcentagemAporteIndividualCorretamente()
        {
            //Arrange
            var valorAporteIndividual = 1000m;
            var valorAporteTotal = 2000m;

            //Act
            var resultado = _service.CalcularPorcentagemAporteIndividual(valorAporteIndividual, valorAporteTotal);

            //Assert
            Assert.Equal(0.5m, resultado);
        }
        /*
        [Fact]
        public void DeveCalcularPorcentagemAporteIndividualZerada()
        {
            //Arrange
            var valorAporteIndividual = 1000m;
            var valorAporteTotal = 2000m;

            //Act
            var resultado = _service.CalcularPorcentagemAporteIndividual(quantidadeAcaoAComprarPorTicker, itemCesta);

            //Assert
            //Lancar execeção
            Assert.Equal(0.5m, resultado);
        }
        */
        [Fact]
        public void DeveCalcularQuantidadeNovaAtivoCorretamente()
        {
            //Arrange
            var quantidadeAcaoAComprarPorTicker = new Dictionary<string, int>
            {
                {"PETR4", 27}
            };

            var itemCesta = new ItemCesta
            {
                Ticker = "PETR4",
                Percentual = 30
            };

            var porcentagemAporteCliente = 0.5m;

            //Act
            var resultado = _service.CalcularQuantidadeNovaAtivo(quantidadeAcaoAComprarPorTicker, itemCesta, porcentagemAporteCliente);

            //Assert
            Assert.Equal(13, resultado);
        }

        [Fact]
        public void CriarOrdemCompraMaster_DeveCriarOrdemPadrao_QuandoQuantidadePadraoMaiorQueZero()
        {
            // Arrange
            var cotacoes = new Dictionary<string, decimal>
            {
                { "PETR4", 30m }
            };

            var ticker = new ItemCesta { Ticker = "PETR4" };
            var data = new DateTime(2025, 1, 10);

            // Act
            var resultado = _service.CriarOrdemCompraMaster(
                100, 0, cotacoes, ticker, data);

            // Assert
            Assert.Single(resultado);

            var ordem = resultado[0];
            Assert.Equal("PETR4", ordem.Ticker);
            Assert.Equal(100, ordem.QuantidadeTotal);
            Assert.Equal(30m, ordem.PrecoUnitario);
            Assert.Equal(3000m, ordem.ValorTotal);
            Assert.Equal("Compra", ordem.TipoOrdem);
            Assert.Equal(data.Date, ordem.DataCompra);
        }

        [Fact]
        public void CriarOrdemCompraMaster_DeveCriarOrdemFracionaria()
        {
            var cotacoes = new Dictionary<string, decimal>
            {
                { "VALE3", 50m }
            };

            var ticker = new ItemCesta { Ticker = "VALE3" };

            var resultado = _service.CriarOrdemCompraMaster(
                0, 5, cotacoes, ticker, DateTime.Today);

            Assert.Single(resultado);

            var ordem = resultado[0];

            Assert.Equal(5, ordem.QuantidadeTotal);
            Assert.Equal("VALE3F", ordem.Detalhes[0].Ticker);
            Assert.Equal("FRACIONARIO", ordem.Detalhes[0].Tipo);
        }

        [Fact]
        public void CriarOrdemCompraMaster_DeveCriarDuasOrdens()
        {
            var cotacoes = new Dictionary<string, decimal>
            {
                { "ITUB4", 25m }
            };

            var ticker = new ItemCesta { Ticker = "ITUB4" };

            var resultado = _service.CriarOrdemCompraMaster(
                100, 3, cotacoes, ticker, DateTime.Today);

            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public void CriarCustodiaMaster_DeveCriarCustodiasBaseadasNasOrdens()
        {
            // Arrange
            var ordens = new List<Ordem>
            {
                new Ordem
                {
                    Ticker = "PETR4",
                    QuantidadeTotal = 10,
                    PrecoUnitario = 30,
                    DataCompra = DateTime.Today
                }
            };

            var conta = new ContaMaster { Id = 1 };

            // Act
            var resultado = _service.CriarCustodiaMaster(ordens, conta);

            // Assert
            Assert.Single(resultado);

            var custodia = resultado[0];

            Assert.Equal("PETR4", custodia.Ticker);
            Assert.Equal(10, custodia.Quantidade);
            Assert.Equal(30, custodia.ValorAtual);
            Assert.Equal(1, custodia.ContaMasterId);
        }

        [Fact]
        public void CriarOuAlterarCustodiaFilhote_DeveCriarNovaCustodia_QuandoAnteriorForNull()
        {
            var cotacoes = new Dictionary<string, decimal>
            {
                { "VALE3", 60m }
            };

            var ticker = new ItemCesta { Ticker = "VALE3" };

            var resultado = _service.CriarCustodiaFilhote(
                null,
                2,
                ticker,
                10,
                60,
                cotacoes,
                DateTime.Today,
                1000);

            Assert.Equal(2, resultado.ContaGraficaId);
            Assert.Equal("VALE3", resultado.Ticker);
            Assert.Equal(10, resultado.Quantidade);
        }

        [Fact]
        public void CriarOuAlterarResiduos_DeveAtualizarResiduoExistente()
        {
            var anterior = new CustodiaMaster { Quantidade = 5 };

            var resultado = _service.CriarOuAlterarResiduos(
                new ItemCesta { Ticker = "ITUB4" },
                10,
                new ContaMaster { Id = 1 },
                DateTime.Today,
                anterior,
                20,
                25);

            Assert.Equal(10, resultado.Quantidade);
        }

        [Fact]
        public void CriarOuAlterarResiduos_DeveCriarNovoResiduo_QuandoNaoExistir()
        {
            var resultado = _service.CriarOuAlterarResiduos(
                new ItemCesta { Ticker = "ITUB4" },
                3,
                new ContaMaster { Id = 1 },
                DateTime.Today,
                null,
                20,
                25);

            Assert.Equal("ITUB4", resultado.Ticker);
            Assert.Equal(3, resultado.Quantidade);
        }

        [Theory]
        [InlineData(100, 90, 10)]
        [InlineData(50, 50, 0)]
        [InlineData(30, 10, 20)]
        public void CalcularQuantidadeResiduo_DeveCalcularCorretamente(
            int totalCompra,
            int distribuido,
            int esperado)
        {
            var resultado = _service.CalcularQuantidadeResiduo(
                totalCompra,
                distribuido);

            Assert.Equal(esperado, resultado);
        }

    }
}