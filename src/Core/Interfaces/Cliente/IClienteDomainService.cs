using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Cliente
{
    public interface IClienteDomainService
    {
        ClienteCadastro criarUsuario(string nome, string cpf, string email, decimal valor, DateTime agora);
        ContaGrafica CriarContaGrafica(ClienteCadastro cliente, DateTime agora);
        void DesativarCliente(ClienteCadastro cliente);
        void AlterarValorMensal(ClienteCadastro cliente, decimal novoValorMensal);
        decimal CalcularTotalInvestido(List<CustodiaFilhote> custodias);
        decimal CalcularValorAtualTotal(List<CustodiaFilhote> custodia);
        
    }
}
