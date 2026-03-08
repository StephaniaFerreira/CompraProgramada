using Aplicacao.Models.Cliente.Adesao;
using Core.Entities;
using Core.Expections;

namespace Aplicacao.Validacoes
{
    public static class ValidacaoCliente
    {
        public static void ValidarAdesao(AdesaoRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new RegraNegocioException("Nome é obrigatório.", "NOME_OBRIGATORIO");

            if (string.IsNullOrWhiteSpace(request.Cpf))
                throw new RegraNegocioException("CPF é obrigatório.", "CPF_OBRIGATORIO");

            if (request.Cpf.Length != 11)
                throw new RegraNegocioException("CPF deve conter 11 caracteres.", "CPF_INVALIDO");

            if (request.ValorMensal < 100)
                throw new RegraNegocioException("O valor mensal minimo e de R$ 100,00.", "VALOR_MENSAL_INVALIDO");
        }

        public static void ValidarAlteracaoValor(decimal novoValor)
        {
            if (novoValor < 100)
                throw new RegraNegocioException("Novo valor deve ser maior ou igual a 100.", "VALOR_MENSAL_INVALIDO");
        }

        public static void ValidarExisteCliente(ClienteCadastro? cliente)
        {
            if (cliente is null)
            {
                throw new RegraNegocioException("Cliente nao encontrado.", "CLIENTE_NAO_ENCONTRADO");
            }
        }
        public static void ValidarCustodiasFilhotes(List<CustodiaFilhote?> custodia)
        {
            if (custodia is null)
            {
                throw new RegraNegocioException("Custodia nao encontrada.", "CUSTODIA_NAO_ENCONTRADA");
            }
        }

        public static void ValidarExisteCPF(bool cpfExiste)
        {
            if (cpfExiste)
            {
                throw new RegraNegocioException("CPF ja cadastrado no sistema", "CLIENTE_CPF_DUPLICADO");
            }
        }
    }
}
