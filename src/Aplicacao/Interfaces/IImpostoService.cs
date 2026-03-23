namespace Aplicacao.Interfaces
{
    public interface IImpostoService
    {
        Task<int> CalcularIRDedoDuro(DateTime dataReferencia);
        void CalcularIRsobreVendas(DateTime data);
    }
}
