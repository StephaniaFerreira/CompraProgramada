using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Cliente;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDbContext _context;

        public ClienteRepository(IDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }
        public bool ExisteCpf(string cpf)
        {
            bool cpfExiste = _context.Clientes
                .Any(c => c.Cpf == cpf);
            return cpfExiste;
        }
        public void AdicionarCliente(ClienteCadastro cliente)
        {
            _context.Clientes.Add(cliente);
        }
        public ClienteCadastro ObterCliente(int ClienteId)
        {
            var cliente = _context.Clientes
                .Include(c => c.ContaGrafica)
                .FirstOrDefault(c => c.Id == ClienteId);
            return cliente;
        }
        public decimal ObterCotacao(string ticker, DateTime dataCriacao)
        {
            var cotacaoHistorica = _context.Cotacoes
                        .Where(c => c.Ticker == ticker && c.DataPregao.Date == dataCriacao)
                        .Select(c => c.PrecoFechamento)
                        .FirstOrDefault();

            if(cotacaoHistorica == 0)
                cotacaoHistorica = _context.Cotacoes
                            .Where(c => c.Ticker == ticker && c.DataPregao.Date <= dataCriacao)
                            .OrderByDescending(c => c.DataPregao)
                            .Select(c => c.PrecoFechamento)
                            .FirstOrDefault();

            return cotacaoHistorica;
        }

        public List<CustodiaFilhote?> ObterCustodiasFilhotes(ClienteCadastro cliente)
        {
            var custodias = _context.CustodiasFilhotes
                .Where(c => c.ContaGraficaId == cliente!.ContaGrafica.Id)
                .ToList();

            return custodias!;
        }
        public List<CustodiaFilhote> ObterCustodiasFilhotesGroupData(ClienteCadastro cliente)
        {
            var aportesPorDia = _context.CustodiasFilhotes
                .Where(c => c.ContaGraficaId == cliente!.ContaGrafica.Id)
                .GroupBy(c => c.DataUltimaAtualizacao.Date)
                .Select(g => g.First())
                .ToList();

            return aportesPorDia;
        }
        public void AdicionarContaGrafica(ContaGrafica conta)
        {
            _context.ContasGraficas.Add(conta);
        }
        public void Salvar()
        {
            _context.SaveChanges();
        }
    }
}
