using Xunit;
using Moq;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using Core.Interfaces;
using Core.Entities;
using Aplicacao.Services;
using Core.Expections;
using Aplicacao.Models.Cliente.Adesao;
using Core.Interfaces.Cliente;

namespace ClienteUnitTests
{
    public class ClienteServiceTests
    {
        [Fact]
        public void Aderir_DeveLancarRegraNegocioException_QuandoValorMensalForMenorQueCem()
        {
            //Arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object);

            var request = new AdesaoRequest("Joao", "12345678901", "joao@email.com", 50.00m);

            //Act
            var exception =  Assert.Throws<RegraNegocioException>(() =>
                service.Aderir(request));
            //Assert
            Assert.Equal("VALOR_MENSAL_INVALIDO", exception.Codigo);
        }

        [Fact]
        public void AderirAoProduto_DeveLancarExcecao_QuandoCpfInvalido()
        {
            // arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object);

            var request = new AdesaoRequest("joao", "123", "joao@email.com", 100.00m);

            // act 
            var exception = Assert.Throws<RegraNegocioException>(() =>
                service.Aderir(request));

            //assert
            Assert.Equal("CPF_INVALIDO", exception.Codigo);
        }

        [Fact]
        public void AderirAoProduto_DeveCriarComSucesso_QuandoDadosValidos()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();

            mockClienteRepository.Setup(c => c.ExisteCpf("456")).Returns(false);

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object);

            var request = new AdesaoRequest("Joao", "12345678945", "joao@email.com", 1000.00m);

            // Act
            var resultado = service.Aderir(request);

            // Assert
            Assert.NotNull(resultado.ContaGrafica);
            Assert.StartsWith("FLH-", resultado.ContaGrafica.NumeroConta);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();


            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        [Fact]
        public void ConsultarCarteira_DeveLancarRegraNegocioException_QuandoClienteNaoExistir()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();

            mockClienteRepository.Setup(c => c.ObterCliente(1)).Returns(new ClienteCadastro());

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object);

            // Act 
            var exception = Assert.Throws<RegraNegocioException>(() =>
                service.ConsultarCarteira(999)); 

            //Assert
            Assert.Equal("CLIENTE_NAO_ENCONTRADO", exception.Codigo);
        }

        [Fact]
        public void ConsultarCarteira_DeveRetornarDadosECalculosCorretos_QuandoClientePossuiCustodia()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();
            int clienteId = 1;

            // 1. Mock do Cliente com sua Conta Grafica
            var conta = new ContaGrafica { Id = 10, NumeroConta = "FLH-000001", ClienteId = clienteId };
            var cliente = new ClienteCadastro
            {
                Id = clienteId,
                Nome = "Joao da Silva",
                ContaGrafica = conta
            };

            var clientesIniciais = new List<ClienteCadastro> { cliente }.AsQueryable();
            var mockSetClientes = CreateMockDbSet(clientesIniciais);
            mockClienteRepository.Setup(c => c.ObterCliente(clienteId)).Returns(cliente);

            var custodias = new List<CustodiaFilhote>
            {
                new CustodiaFilhote
                {
                    ContaGraficaId = 10,
                    Ticker = "PETR4",
                    Quantidade = 10,
                    PrecoMedio = 30.00m, // Investido: 300
                    ValorAtual = 33.00m   // Atual: 330 (PL: +30 / +10%)
                },
                new CustodiaFilhote
                {
                    ContaGraficaId = 10,
                    Ticker = "VALE3",
                    Quantidade = 5,
                    PrecoMedio = 100.00m, // Investido: 500
                    ValorAtual = 90.00m    // Atual: 450 (PL: -50 / -10%)
                }
            }.ToList();

            mockClienteRepository.Setup(c => c.ObterCustodiasFilhotes(cliente)).Returns(custodias!);

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object); ;

            // Act
            var resultado = service.ConsultarCarteira(clienteId);

            // Assert
            Assert.Equal(clienteId, resultado.ClienteId);
            Assert.Equal("Joao da Silva", resultado.Nome);

            // Validação do Resumo Geral
            // Total Investido: 300 + 500 = 800
            // Total Atual: 330 + 450 = 780
            // PL Total: -20
            Assert.Equal(800m, resultado.Resumo.ValorTotalInvestido);
            Assert.Equal(780m, resultado.Resumo.ValorAtualCarteira);
            Assert.Equal(-20m, resultado.Resumo.PlTotal);
            Assert.Equal(-2.5m, resultado.Resumo.RentabilidadePercentual); // (780/800 - 1) * 100

            // Validação de um Ativo específico (PETR4)
            var petr4 = resultado.Ativos.FirstOrDefault(a => a.Ticker == "PETR4");
            Assert.NotNull(petr4);
            Assert.Equal(330m, petr4!.ValorAtual);
            Assert.Equal(30m, petr4.Pl);
            Assert.Equal(10m, petr4.PlPercentual);
        }

        [Fact]
        public void ConsultarRentabilidade_DeveRetornarDadosCompletos_QuandoClientePossuiHistorico()
        {
            //Arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();
            int clienteId = 1;
            int contaId = 10;

            var cliente = new ClienteCadastro { Id = clienteId, Nome = "Joao da Silva", ContaGrafica = new ContaGrafica { Id = contaId } };
            mockClienteRepository.Setup(c => c.ObterCliente(clienteId)).Returns(cliente);


            var custodias = new List<CustodiaFilhote>
            {
                new CustodiaFilhote { ContaGraficaId = contaId, Quantidade = 100, PrecoMedio = 60.00m, ValorAtual = 64.50m } // Investido: 6000 | Atual: 6450
            }.ToList();
            mockClienteRepository.Setup(c => c.ObterCustodiasFilhotes(cliente)).Returns(custodias!);

            // 3. Mock das Execuções (Histórico de Aportes)
            var execucoes = new List<DistribuicaoCliente>
            {
                new DistribuicaoCliente { ClienteId = clienteId, DataCriacao = new DateTime(2026, 01, 05), ValorAporte = 1000.00m },
                new DistribuicaoCliente { ClienteId = clienteId, DataCriacao = new DateTime(2026, 01, 15), ValorAporte = 1000.00m }
            }.ToList();
            
            mockClienteRepository.Setup(c => c.ObterDistribuicoesCliente(clienteId)).Returns(execucoes);

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object);

            // Act
            var resultado = service.ObterRentabilidadeDetalhada(clienteId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Joao da Silva", resultado.Nome);

            // Validação do Resumo de Rentabilidade
            Assert.Equal(6000.00m, resultado.Rentabilidade.ValorTotalInvestido);
            Assert.Equal(6450.00m, resultado.Rentabilidade.ValorAtualCarteira);
            Assert.Equal(450.00m, resultado.Rentabilidade.PlTotal);
            Assert.Equal(7.50m, resultado.Rentabilidade.RentabilidadePercentual);

            // Validação do Histórico 
            Assert.Equal(2, resultado.HistoricoAportes.Count);
            //Assert.Equal("1/3", resultado.HistoricoAportes[0].Parcela);
            Assert.Equal(1000.00m, resultado.HistoricoAportes[0].Valor);
        }

        [Fact]
        public void ConsultarRentabilidade_DeveLancarExcecao_QuandoClienteNaoExistir()
        {
            // Arrange
            var mockClienteDomainService = new Mock<IClienteDomainService>();
            var mockClienteRepository = new Mock<IClienteRepository>();
            mockClienteRepository.Setup(c => c.ObterCliente(1)).Returns(new ClienteCadastro());

            var service = new ClienteService(mockClienteDomainService.Object, mockClienteRepository.Object);

            // Act & Assert
            var exception = Assert.Throws<RegraNegocioException>(() => service.ObterRentabilidadeDetalhada(99));
            //Assert
            Assert.Equal("CLIENTE_NAO_ENCONTRADO", exception.Codigo);
        }
    }
}