using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Backoffice;
using Microsoft.EntityFrameworkCore;


namespace Core.Repositories
{
    public class BackofficeRepository : IBackofficeRepository
    {
        private readonly IDbContext _context;

        public BackofficeRepository(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        public Cesta? ObterCestaAtual()
        {
            var cestaAtual = _context.Cestas
                .Include(c => c.Itens)
                .FirstOrDefault(c => c.Ativa);

            return cestaAtual;
        }
        public List<Cesta> ObterCestas()
        {
            var cestas = _context.Cestas
                .Include(c => c.Itens)
                .OrderByDescending(c => c.DataCriacao)
                .ToList();

            return cestas;
        }
        public ContaMaster ObterContaMaster()
        {
            return _context.ContaMaster!.FirstOrDefault()!;
        }
        public List<Ordem> ObterOrdens(DateTime data)
        {
            return _context.Ordens.Where(o => o.DataCompra.Date == data.Date).ToList();
        }
        public List<CustodiaMaster> ObterCustodiaMaster()
        {
            return _context.CustodiaMaster!.Where(i => i.Quantidade != 0).ToList();
        }
        public List<CustodiaFilhote> ObterCustodiaFilhotes(DateTime data)
        {
            return _context.CustodiasFilhotes.Where(c => c.DataUltimaAtualizacao.Date == data.Date).ToList();
        }
        public void AdicionarCesta(Cesta cesta)
        {
            _context.Cestas.Add(cesta);
        }
        public void Salvar()
        {
            _context.SaveChanges();
        }
        public int ObterTotalClientesAtivos()
        {
            return _context.Clientes.Count(c => c.Ativo);
        }
        public List<ClienteCadastro> ObterClientesAtivos()
        {
            return _context.Clientes
                           .Include(c => c.ContaGrafica)
                           .Where(c => c.Ativo).ToList();
        }

        public Dictionary<string,decimal> ObterCotacaoPorTicker(List<ItemCesta> item)
        {
            var tickersInteresse = item.Select(i => i.Ticker).ToList();


            return _context.Cotacoes
                .Where(c => tickersInteresse.Contains(c.Ticker))
                .ToDictionary(
                    c => c.Ticker,          
                    c => c.PrecoFechamento  
                );
        }




    }
}
