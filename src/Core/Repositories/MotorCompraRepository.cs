using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.MotorCompra;
using Microsoft.EntityFrameworkCore;


namespace Core.Repositories
{
    public class MotorCompraRepository: IMotorCompraRepository
    {
        private readonly IDbContext _context;

        public MotorCompraRepository(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        public List<ClienteCadastro> ObterClientesAtivos()
        {
            return _context.Clientes.Where(c => c.Ativo).ToList();
        }
        public void Salvar()
        {
            _context.SaveChanges();
        }
        public void AdicionarResiduos(CustodiaMaster residuo)
        {
            if (residuo.Id == 0) 
            {
                _context.CustodiaMaster.Add(residuo);
            }
            else 
            {
                ((DbContext)_context).Entry(residuo).State = EntityState.Modified;
            }
        }
        public CustodiaMaster? ObterResiduoMaster(ContaMaster contaMaster, ItemCesta Ticker)
        {
            return contaMaster!.ItensCustodia!.FirstOrDefault(i => i.Ticker == Ticker.Ticker);
        }

        public CustodiaFilhote ObterCustodiaFilhote(ItemCesta Ticker, ClienteCadastro cliente)
        {
            var custodiaAnterior = _context.CustodiasFilhotes
                                                .FirstOrDefault(c =>
                                                                c.Ticker == Ticker.Ticker &&
                                                                c.ContaGrafica!.Cliente!.Id == cliente.Id);
            return custodiaAnterior!;
        }
        public int ObterContaGraficaId(ItemCesta Ticker, ClienteCadastro cliente)
        {
            return _context.ContasGraficas.Where(i => i.ClienteId == cliente.Id).Select(c => c.Id).FirstOrDefault();
        }
        public void AdicionarCustodiaFilhote(CustodiaFilhote custodia)
        {
            if (custodia.Id == 0)
            {
                _context.CustodiasFilhotes.Add(custodia);
            }
            else
            {
                ((DbContext)_context).Entry(custodia).State = EntityState.Modified;
            }
        }
        public void AdicionarCustodiaMaster(List<CustodiaMaster> custodias)
        {
            _context.CustodiaMaster.AddRange(custodias);
        }
        public int ObterQuantidadeRemanecenteCustodia(ContaMaster contaMaster, ItemCesta Ticker)
        {
            return contaMaster!.ItensCustodia.Where(i => i.Ticker == Ticker.Ticker && i.Quantidade > 0).Select(i => i.Quantidade).FirstOrDefault();
        }
        public void AdicionarOrdensMaster(List<Ordem> ordens)
        {
            _context.Ordens.AddRange(ordens);
        }
        public Cesta ObterCestaVigente()
        {
            var cestaVigente = _context.Cestas
                                .Include(c => c.Itens)
                                .OrderByDescending(c => c.DataCriacao)
                                .FirstOrDefault();
            return cestaVigente!;
        } 
        public ContaMaster ObterContaMaster()
        {
            var contaMaster = _context.ContaMaster
                                .Include(c => c.ItensCustodia)
                                .FirstOrDefault(c => c.Tipo == "MASTER");
            return contaMaster!;
        }
        public Dictionary<string, decimal> ObterCotacaoPorTicker(List<ItemCesta> item, DateTime data)
        {
            var TickersInteresse = item.Select(i => i.Ticker).ToList();


            return _context.Cotacoes
                .Where(c => TickersInteresse.Contains(c.Ticker) && c.DataRegistro.Date == data.Date)
                .ToDictionary(
                    c => c.Ticker,
                    c => c.PrecoFechamento
                );
        }
    }
}
