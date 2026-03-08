using Core.Entities;

namespace Aplicacao.Models.Cesta
{
    public class CustodiaMasterResponse
    {
        public ContaMaster ContaMaster { get; set; } = null!;
        public List<CustodiaMaster> Custodia { get; set; } = new();
        public decimal ValorTotalResiduo { get; set; }
    }
}
