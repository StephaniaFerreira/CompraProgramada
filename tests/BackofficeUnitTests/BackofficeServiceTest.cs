using Moq;
using System.Collections.Generic;
using Xunit;
using Core.MotorCompra;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Core.Interfaces;
using Aplicacao.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Aplicacao.Models.Cesta;
using Aplicacao.Services;
using Core.Interfaces.Backoffice;
using System.Threading.Tasks;

namespace BackofficeUnitTests
{
    public class BackofficeServiceTest
    {
        [Fact]
        public void SalvarCesta_DeveLancarExcecao_QuandoSomaPercentuaEhDiferenteDeCem()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);

            var request = new CestaRequest(
                "Cesta Invalida",
                new List<ItemRequest> {
            new ("PETR4", 20.00m), new ("VALE3", 20.00m),
            new ("ITUB4", 20.00m), new ("BBDC4", 20.00m),
            new ("WEGE3", 10.00m)
                }
            );

            // Act & Assert
            var exception = Assert.Throws<CestaException>(() => service.CadastrarOuAlterar(request));
            Assert.Equal("PERCENTUAIS_INVALIDOS", exception.Codigo);
        }

        [Fact]
        public void SalvarCesta_DeveLancarExcecao_QuandoNaoHouverExatamenteCincoAtivos()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);

            var request = new CestaRequest(
                "Cesta Incompleta",
                new List<ItemRequest> {
            new ("PETR4", 50.00m), new ("VALE3", 50.00m)
                }
            );

            // Act & Assert
            var exception = Assert.Throws<CestaException>(() => service.CadastrarOuAlterar(request));
            Assert.Equal("QUANTIDADE_ATIVOS_INVALIDA", exception.Codigo);
        }

        [Fact]
        public void SalvarCesta_DeveDesativarAnterior_QuandoJaExistirCestaAtiva()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);

          
            var cestaAnterior = new Cesta
            {
                Id = 1,
                Nome = "Fevereiro",
                Ativa = true,
                Itens = new List<ItemCesta> {
                    new() { Ticker = "PETR4" }, new() { Ticker = "VALE3" },
                    new() { Ticker = "ITUB4" }, new() { Ticker = "BBDC4" },
                    new() { Ticker = "WEGE3" }
                }
            };

            mockClienteRepository.Setup(c => c.ObterCestaAtual()).Returns(cestaAnterior);

            //Nova Cesta trocando BBDC4 e WEGE3 por ABEV3 e RENT3
            var request = new CestaRequest(
                "Março",
                new List<ItemRequest> {
            new ("PETR4", 20.00m), new ("VALE3", 20.00m),
            new ("ITUB4", 20.00m), new ("ABEV3", 20.00m),
            new ("RENT3", 20.00m)
                }
            );

            // Act
            var resultado = service.CadastrarOuAlterar(request);

            // Assert
            Assert.True(resultado.RebalanceamentoDisparado);
            Assert.False(cestaAnterior.Ativa); // Garante que a velha foi desativada
            Assert.Contains("ABEV3", resultado.AtivosAdicionados);
            Assert.Contains("BBDC4", resultado.AtivosRemovidos);
        }

        [Fact]
        public void ConsultarCestaAtual_DeveLancarExcecao_QuandoNaoHouverCestaAtiva()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);

            var cesta = new Cesta
            {
                Id = 2,
                Nome = "Top Five - Marco 2026",
                Ativa = false,
                DataCriacao = new DateTime(2026, 03, 01, 9, 0, 0, DateTimeKind.Utc),
                Itens = new List<ItemCesta>
                {
                    new ItemCesta { Ticker = "PETR4", Percentual = 25.00m },
                    new ItemCesta { Ticker = "VALE3", Percentual = 20.00m, CotacaoAtual = 65.00m },
                    new ItemCesta { Ticker = "ITUB4", Percentual = 20.00m },
                    new ItemCesta { Ticker = "ABEV3", Percentual = 20.00m },
                    new ItemCesta { Ticker = "RENT3", Percentual = 15.00m }
                }
            };


            mockClienteRepository.Setup(c => c.ObterCestaAtual()).Returns(cesta);

            // Act 
            var exception = Assert.Throws<CestaException>(() => service.ConsultarAtual());

            //Assert
            Assert.Equal("CESTA_NAO_ENCONTRADA", exception.Codigo);
        }


        [Fact]
        public void ConsultarAtual_DeveRetornarDadosCorretos_QuandoCestaAtivaExistir()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);

            var cestaAtiva = new Cesta
            {
                Id = 2,
                Nome = "Top Five - Marco 2026",
                Ativa = true,
                DataCriacao = new DateTime(2026, 03, 01, 9, 0, 0, DateTimeKind.Utc),
                Itens = new List<ItemCesta>
                {
                    new ItemCesta { Ticker = "PETR4", Percentual = 25.00m },
                    new ItemCesta { Ticker = "VALE3", Percentual = 20.00m, CotacaoAtual = 65.00m },
                    new ItemCesta { Ticker = "ITUB4", Percentual = 20.00m },
                    new ItemCesta { Ticker = "ABEV3", Percentual = 20.00m },
                    new ItemCesta { Ticker = "RENT3", Percentual = 15.00m }
                }
            };


            mockClienteRepository.Setup(c => c.ObterCestaAtual()).Returns(cestaAtiva);

            var cotacoesFake = new Dictionary<string, decimal>
                    {
                        { "PETR4", 37.00m },
                        { "ITUB4", 31.00m },
                        { "ABEV3", 14.50m },
                        { "RENT3", 49.00m }
                    };
            mockClienteRepository.Setup(s => s.ObterCotacaoPorTicket(cestaAtiva.Itens)).Returns(cotacoesFake);

            // Act
            var response = service.ConsultarAtual();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.CestaId);
            Assert.Equal("Top Five - Marco 2026", response.Nome);
            Assert.True(response.Ativa);
            Assert.Equal(5, response.Itens.Count);

            // Verificação de Precisão Numérica (Decimal)
            var petr4 = response.Itens.First(i => i.Ticker == "PETR4");
            Assert.Equal(25.00m, petr4.Percentual);
            Assert.Equal(37.00m, petr4.CotacaoAtual);

            var vale3 = response.Itens.First(i => i.Ticker == "VALE3");
            Assert.Equal(65.00m, vale3.CotacaoAtual);

            Assert.Equal(100.00m, response.Itens.Sum(i => i.Percentual));
        }

        [Fact]
        public void ListarHistorico_DeveRetornarListaDeCestasOrdenada()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);


            var dataMarço = new DateTime(2026, 03, 01, 9, 0, 0, DateTimeKind.Utc);
            var dataFevereiro = new DateTime(2026, 02, 01, 9, 0, 0, DateTimeKind.Utc);

            var listaCestas = new List<Cesta>
            {
                new Cesta {
                    Id = 2, Nome = "Top Five - Marco 2026", Ativa = true, DataCriacao = dataMarço,
                    Itens = new List<ItemCesta> { new ItemCesta { Ticker = "PETR4", Percentual = 25.00m } }
                },
                new Cesta {
                    Id = 1, Nome = "Top Five - Fevereiro 2026", Ativa = false, DataCriacao = dataFevereiro, DataDesativacao = dataMarço,
                    Itens = new List<ItemCesta> { new ItemCesta { Ticker = "PETR4", Percentual = 30.00m } }
                }
            }.ToList();


            mockClienteRepository.Setup(c => c.ObterCestas()).Returns(listaCestas);

            // Act
            var response = service.Historico();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(2, response.Cestas.Count);
            Assert.Equal(2, response.Cestas.First().CestaId); // Verifica se a mais recente (Id 2) veio primeiro
            Assert.Null(response.Cestas.First().DataDesativacao);
            Assert.NotNull(response.Cestas.Last().DataDesativacao);
        }

        [Fact]
        public void ConsultarCustodiaMaster_DeveCalcularValoresCorretamente()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IBackofficeDomainService>();
            var mockClienteRepository = new Mock<IBackofficeRepository>();
            var mockMotorCompra = new Mock<IMotorCompraService>();
            var service = new BackofficeService(mockClienteDomainService.Object, mockClienteRepository.Object, mockMotorCompra.Object);

            // 1. Mock da Conta Master
            var contaMaster = new  ContaMaster { Id = 1, NumeroConta = "MST-000001", Tipo = "MASTER" };

            // 2. Mock da Custódia (Resíduos)
            var itensCustodia = new List<CustodiaMaster>
            {
                new CustodiaMaster { Ticker = "PETR4", Quantidade = 1, ValorAtual = 37.00m, PrecoMedio = 35.00m, Origem = "Residuo distribuicao 2026-02-05" },
                new CustodiaMaster { Ticker = "ITUB4", Quantidade = 1, ValorAtual = 31.00m,PrecoMedio = 30.00m, Origem = "Residuo distribuicao 2026-02-05" },
                new CustodiaMaster { Ticker = "WEGE3", Quantidade = 1, ValorAtual = 42.00m,PrecoMedio = 40.00m, Origem = "Residuo distribuicao 2026-02-05" }
            };

            //var queryConta = contaMaster.AsQueryable();
            //var queryCustodia = itensCustodia.AsQueryable();

            //// 2. Crie o Mock do DbSet para ContaMaster
            //var mockDbSetConta = new Mock<DbSet<ContaMaster>>();
            //mockDbSetConta.As<IQueryable<ContaMaster>>().Setup(m => m.Provider).Returns(queryConta.Provider);
            //mockDbSetConta.As<IQueryable<ContaMaster>>().Setup(m => m.Expression).Returns(queryConta.Expression);
            //mockDbSetConta.As<IQueryable<ContaMaster>>().Setup(m => m.ElementType).Returns(queryConta.ElementType);
            //mockDbSetConta.As<IQueryable<ContaMaster>>().Setup(m => m.GetEnumerator()).Returns(queryConta.GetEnumerator());

            //// 3. Crie o Mock do DbSet para CustodiaMaster
            //var mockDbSetCustodia = new Mock<DbSet<CustodiaMaster>>();
            //mockDbSetCustodia.As<IQueryable<CustodiaMaster>>().Setup(m => m.Provider).Returns(queryCustodia.Provider);
            //mockDbSetCustodia.As<IQueryable<CustodiaMaster>>().Setup(m => m.Expression).Returns(queryCustodia.Expression);
            //mockDbSetCustodia.As<IQueryable<CustodiaMaster>>().Setup(m => m.ElementType).Returns(queryCustodia.ElementType);
            //mockDbSetCustodia.As<IQueryable<CustodiaMaster>>().Setup(m => m.GetEnumerator()).Returns(queryCustodia.GetEnumerator());

            // 4. Agora faça o Setup das propriedades do Contexto
            mockClienteRepository.Setup(c => c.ObterContaMaster()).Returns(contaMaster!);
            mockClienteRepository.Setup(c => c.ObterCustodiaMaster()).Returns(itensCustodia);


            // 3. Mock das Cotações Atuais
            var cotacoesAtuais = new Dictionary<string, decimal>
            {
                { "PETR4", 37.00m },
                { "ITUB4", 31.00m },
                { "WEGE3", 42.00m }
            };
            mockClienteRepository.Setup(s => s.ObterCotacaoPorTicket(new List<ItemCesta>())).Returns(cotacoesAtuais);

            // Act
            var response = service.ConsultarCustodiaMaster();

            // Assert
            Assert.Equal("MST-000001", response.ContaMaster.NumeroConta);
            Assert.Equal(3, response.Custodia.Count);

            // Validação do Cálculo do Valor Total (precoMedio * Quantidade)
            Assert.Equal(110.00m, response.ValorTotalResiduo);

            // Validação de um item específico
            var petr4 = response.Custodia.First(x => x.Ticker == "PETR4");
            Assert.Equal(37.00m, petr4.ValorAtual);
        }

        [Fact]
        public void DeveExecutarMotorCompraCom3Clientes()
        {
            //// Arrange
            //// 1. Mocks das dependências (Repositórios e Serviços)
            //var mockMotorService = new Mock<IMotorCompraService>();
            //var mockCotacaoService = new Mock<ICotacaoService>();
            //var mockImpostoService = new Mock<IImpostoService>();

            //// 2. Setup dos dados de entrada (Simulando o que viria do banco)
            //var clientes = new List<ClienteCadastro>
            //{
            //    new { Id = 1, Nome = "Joao", ValorMensal = 3000, Ativo = true, ContaGrafica = new ContaGrafica { Id = 1 } },
            //    new { Id = 2, Nome = "Maria", ValorMensal = 6000, Ativo = true, ContaGrafica = new ContaGrafica { Id = 2 } },
            //    new { Id = 3, Nome = "Pedro", ValorMensal = 1500, Ativo = true, ContaGrafica = new ContaGrafica { Id = 3 } }
            //};

            //var cesta = new Cesta
            //{
            //    Id = 1,
            //    Itens = new List<ItemCesta>
            //    {
            //        new() { Ticker = "PETR4", Percentual = 0.30m },
            //        new() { Ticker = "VALE3", Percentual = 0.25m }
            //        // ... adicione os outros tickers
            //    }
            //};


            //mockMotorService.Setup(r => r.O).ReturnsAsync(clientes);
            //mockMotorServicev.Setup(r => r.ObterCestaVigente()).ReturnsAsync(cesta);

            //mockCotacaoService.Setup(x => x.ObterCotacoes())
            //    .Returns(new Dictionary<string, decimal> { { "PETR4", 35 }, { "VALE3", 62 } });

            //var service = new MotorCompraService(
            //    mockRepository.Object,
            //    mockCotacaoService.Object,
            //    mockImpostoService.Object
            //);

            //// Act
            //await service.ExecutarMotorDeCompra(new DateTime(2026, 2, 5));

            //// Assert
            //// Verificamos se o repositório foi chamado para salvar as ordens e custódias
            //mockRepository.Verify(r => r.SalvarOrdens(It.Is<List<OrdemCompra>>(l => l.Count > 0)), Times.Once);

            //impostoMock.Verify(
            //    x => x.CalcularIRDedoDuro(It.IsAny<DateTime>()),
            //    Times.Once
            //);
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();


            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}


/*

 * [Fact]
    public async Task NaoDeveExecutarSemClientes()

    [Fact]
public async Task DeveFalharSemCestaVigente()

[Fact]
public async Task DeveDescontarSaldoDaContaMaster()

*/