using Core.Entities;
using Core.Interfaces.Cliente;

namespace Core.Service
{
    public class ClienteDomainService: IClienteDomainService
    {
        public ContaGrafica CriarContaGrafica(ClienteCadastro cliente, DateTime agora)
        {
            var conta = new ContaGrafica
            {
                
                NumeroConta = $"FLH-{cliente.Id:D6}",
                Tipo = "FILHOTE",
                DataCriacao = agora,
                ClienteId = cliente.Id
            };
            return conta;
        }
        public ClienteCadastro criarUsuario(string nome, string cpf, string email, decimal valor, DateTime agora)
        {
            var cliente = new ClienteCadastro
            {
                Nome = nome,
                Cpf = cpf,
                Email = email,
                ValorMensal = valor,
                Ativo = true,
                DataAdesao = agora
            };
            return cliente;
        }

        public void DesativarCliente(ClienteCadastro cliente)
        {
            cliente!.Ativo = false;
            cliente.DataSaida = DateTime.UtcNow;
        }

        public void AlterarValorMensal(ClienteCadastro cliente, decimal novoValorMensal)
        {
            cliente.ValorMensal = novoValorMensal;
        }
        public decimal CalcularTotalInvestido(List<CustodiaFilhote> custodias)
        {
            return custodias.Sum(x => x.Quantidade * x.PrecoMedio);
        } 
        public decimal CalcularValorAtualTotal(List<CustodiaFilhote> custodias)
        {
            return custodias.Sum(x => x.Quantidade * x.ValorAtual);
        }
    }
}
