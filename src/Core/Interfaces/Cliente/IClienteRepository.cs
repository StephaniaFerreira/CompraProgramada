using Core.Entities;

namespace Core.Interfaces.Cliente
{
    public interface IClienteRepository
    {
        bool ExisteCpf(string cpf);
        void Salvar();
        void AdicionarCliente(ClienteCadastro cliente);
        void AdicionarContaGrafica(ContaGrafica cliente);
        ClienteCadastro ObterCliente(int clienteId);
        List<DistribuicaoCliente> ObterDistribuicoesCliente(int clienteId);
        List<CustodiaFilhote?> ObterCustodiasFilhotes(ClienteCadastro cliente);
        decimal ObterCotacao(AtivoDistribuido ativo, DateTime dataCriacao);



    }
}
