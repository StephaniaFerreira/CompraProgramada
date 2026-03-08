using Aplicacao.Models.Cesta;

namespace Aplicacao.Interfaces;

public interface IBackofficeService
{
    CestaResponse CadastrarOuAlterar(CestaRequest request);
    CestaAtualResponse ConsultarAtual();
    HistoricoCestaResponse Historico();
    CustodiaMasterResponse ConsultarCustodiaMaster();
    ExecucaoCompraResponse ExecutarCompraMotor(ExecucaoCompraRequest request);
}
