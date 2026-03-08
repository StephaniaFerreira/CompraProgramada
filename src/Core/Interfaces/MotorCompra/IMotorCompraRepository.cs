using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.MotorCompra
{
    public interface IMotorCompraRepository
    {
        List<ClienteCadastro> ObterClientesAtivos();
        Cesta ObterCestaVigente();
        ContaMaster ObterContaMaster();
        Dictionary<string, decimal> ObterCotacaoPorTicket(List<ItemCesta> item, DateTime data);
        int ObterQuantidadeRemanecenteCustodia(ContaMaster conta, ItemCesta item);
        int ObterContaGraficaId(ItemCesta item, ClienteCadastro cliente);
        void AdicionarOrdensCompraMaster(List<OrdemCompra> ordens);
        void AdicionarCustodiaFilhote(CustodiaFilhote custodia);
        CustodiaFilhote ObterCustodiaFilhote(ItemCesta ticket, ClienteCadastro cliente);
        CustodiaMaster? ObterResiduoMaster(ContaMaster contaMaster, ItemCesta ticket);
        void Salvar();
        void AdicionarResiduos(CustodiaMaster residuo);
    }
}
