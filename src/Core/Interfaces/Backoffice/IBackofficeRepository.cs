using Core.Entities;

namespace Core.Interfaces.Backoffice
{
    public interface IBackofficeRepository
    {
        Cesta? ObterCestaAtual();
        List<Cesta> ObterCestas();
        int ObterTotalClientesAtivos();
        ContaMaster ObterContaMaster();
        List<CustodiaMaster> ObterCustodiaMaster();
        void AdicionarCesta(Cesta cesta);
        void Salvar();
        Dictionary<string, decimal> ObterCotacaoPorTicket(List<ItemCesta> item);


    }
}
