namespace Aplicacao.Interfaces
{
    public interface IMotorCompraService
    {
        void ExecutarMotorDeCompra(DateTime data);
        DateTime ObterProximoDiaUtil(DateTime data);
    }
}
