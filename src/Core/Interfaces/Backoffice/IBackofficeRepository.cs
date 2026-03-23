using Core.Entities;

namespace Core.Interfaces.Backoffice
{
    public interface IBackofficeRepository
    {
        Cesta? ObterCestaAtual();
        List<Cesta> ObterCestas();
        int ObterTotalClientesAtivos();
        ContaMaster ObterContaMaster();
        List<CustodiaFilhote> ObterCustodiaFilhotes(DateTime data);
        List<CustodiaMaster> ObterCustodiaMaster();
        void AdicionarCesta(Cesta cesta);
        void Salvar();
        Dictionary<string, decimal> ObterCotacaoPorTicker(List<ItemCesta> item);
        List<Ordem> ObterOrdens(DateTime data);

        List<ClienteCadastro> ObterClientesAtivos();

    }
}
