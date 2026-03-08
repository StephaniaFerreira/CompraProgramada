namespace Aplicacao.Interfaces
{
    public interface IImpostoService
    {
        Task CalcularIRDedoDuro(DateTime dataReferencia);
        void CalcularIRsobreVendas(DateTime data);
    }
}
