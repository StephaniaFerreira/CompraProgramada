using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Cotacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class CotacaoRepository : ICotacaoRepository
    {
        private readonly IDbContext _context;

        public CotacaoRepository(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public void PopularTabela(List<Cotacao> cotacoes)
        {
            _context.Cotacoes.AddRange(cotacoes);
            _context.SaveChanges();

        }

        public bool ExisteRegistroParaData(DateTime data)
        {
            return _context.Cotacoes.Any(c => c.DataRegistro == data.Date);
        }
        public List<string> ObterTicketsCestaAtual()
        {
            return _context.Cestas
                            .OrderByDescending(c => c.DataCriacao)
                            .Take(1)
                            .SelectMany(c => c.Itens)
                            .Select(i => i.Ticker).ToList();
        }
    }
}
