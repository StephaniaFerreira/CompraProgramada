using Xunit;
using System.Collections.Generic;
using System;
using Core.Entities;
using Core.Service;
using Core.Expections;

namespace ClienteUnitTests
{
    public class ClienteServiceTests
    {
        private readonly ClienteDomainService _domainService;

        public ClienteServiceTests()
        {
            _domainService = new ClienteDomainService();
        }

        [Fact]
        public void CriarContaGrafica_DeveCriarContaComDadosCorretos()
        {
            // Arrange
            var cliente = new ClienteCadastro { Id = 12 };
            var agora = new DateTime(2025, 01, 10);

            // Act
            var conta = _domainService.CriarContaGrafica(cliente, agora);

            // Assert
            Assert.Equal("FLH-000012", conta.NumeroConta);
            Assert.Equal("FILHOTE", conta.Tipo);
            Assert.Equal(agora, conta.DataCriacao);
            Assert.Equal(cliente.Id, conta.ClienteId);
        }


        [Fact]
        public void CriarUsuario_DeveCriarClienteAtivoComDadosInformados()
        {
            // Arrange
            var agora = DateTime.Today;

            // Act
            var cliente = _domainService.criarUsuario(
                "joao",
                "12345678900",
                "teste@email.com",
                3000,
                agora);

            // Assert
            Assert.Equal("joao", cliente.Nome);
            Assert.Equal("12345678900", cliente.Cpf);
            Assert.Equal("teste@email.com", cliente.Email);
            Assert.Equal(3000, cliente.ValorMensal);
            Assert.True(cliente.Ativo);
            Assert.Equal(agora, cliente.DataAdesao);
        }

        

        [Fact]
        public void DesativarCliente_DeveDesativarClienteEPreencherDataSaida()
        {
            // Arrange
            var cliente = new ClienteCadastro
            {
                Ativo = true
            };

            // Act
            _domainService.DesativarCliente(cliente);

            // Assert
            Assert.False(cliente.Ativo);
            Assert.NotNull(cliente.DataSaida);
        }

        [Fact]
        public void AlterarValorMensal_DeveAtualizarValor()
        {
            // Arrange
            var cliente = new ClienteCadastro
            {
                ValorMensal = 3000
            };

            // Act
            _domainService.AlterarValorMensal(cliente, 6000);

            // Assert
            Assert.Equal(6000, cliente.ValorMensal);
        }

        

        [Fact]
        public void CalcularTotalInvestido_DeveSomarQuantidadePorPrecoMedio()
        {
            // Arrange
            var custodias = new List<CustodiaFilhote>
            {
                new CustodiaFilhote { Quantidade = 10, PrecoMedio = 20 },
                new CustodiaFilhote { Quantidade = 5, PrecoMedio = 30 }
            };

            // Act
            var total = _domainService.CalcularTotalInvestido(custodias);

            // Assert
            Assert.Equal(350, total); // (10*20) + (5*30)
        }

        [Fact]
        public void CalcularValorAtualTotal_DeveSomarQuantidadePorValorAtual()
        {
            // Arrange
            var custodias = new List<CustodiaFilhote>
            {
                new CustodiaFilhote { Quantidade = 10, ValorAtual = 25 },
                new CustodiaFilhote { Quantidade = 2, ValorAtual = 50 }
            };

            // Act
            var total = _domainService.CalcularValorAtualTotal(custodias);

            // Assert
            Assert.Equal(350, total); // (10*25) + (2*50)
        }

        [Fact]
        public void CalcularTotalInvestido_DeveRetornarZero_QuandoListaVazia()
        {
            // Arrange
            var custodias = new List<CustodiaFilhote>();

            // Act
            var total = _domainService.CalcularTotalInvestido(custodias);

            // Assert
            Assert.Equal(0, total);
        }

    }
}