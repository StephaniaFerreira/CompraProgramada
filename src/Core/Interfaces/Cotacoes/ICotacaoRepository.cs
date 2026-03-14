using Core.Entities;

namespace Core.Interfaces.Cotacoes
{
    public interface ICotacaoRepository
    {
        void PopularTabela(List<Cotacao> cotacoes);
        bool ExisteRegistroParaData(DateTime data);
        List<string> ObterTicketsCestaAtual();
    }
}
