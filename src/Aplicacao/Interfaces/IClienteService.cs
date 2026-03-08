using Aplicacao.Models.Cliente.Adesao;
using Aplicacao.Models.Cliente.AlterarValorMensal;
using Aplicacao.Models.Cliente.Carteira;

using Cliente.Models.Saida;

namespace Aplicacao.Interfaces
{
    public interface IClienteService
    {
        AdesaoResponse Aderir(AdesaoRequest request);
        SaidaResponse Sair(int clienteId);
        AlterarValorMensalResponse AlterarValorMensal(int clienteId, AlterarValorMensalRequest request);
        CarteiraResponse ConsultarCarteira(int clienteId);
        RentabilidadeDetalhadaResponse ObterRentabilidadeDetalhada(int clienteId);
    }
}
