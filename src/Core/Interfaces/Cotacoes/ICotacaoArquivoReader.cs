using Core.Entities;

namespace Core.Interfaces.Cotacoes
{
    public interface ICotacaoArquivoReader
    {
        Task<List<Cotacao>> LerAsync(DateTime data);
    }
}
