using Microsoft.EntityFrameworkCore;
using Aplicacao.Interfaces;
using Core.Interfaces;

namespace Aplicacao.Services
{
    public class MotorRebalanceamentoService : IMotorRebalanceamentoService
    {
        private readonly IDbContext _context;
        private readonly IMotorCompraService _motorCompra;
        private readonly IImpostoService _impostoService;

        public MotorRebalanceamentoService(IDbContext context, IMotorCompraService motorCompra,IImpostoService impostoService)
        {
            _context = context;
            _motorCompra = motorCompra;
            _impostoService = impostoService;
        }
        public void ExecutarMotorDeRebalanceamento(DateTime data)
        {
            var cestaVigente = _context.Cestas
                                .Include(c => c.Itens)
                                .OrderByDescending(c => c.DataCriacao)
                                .FirstOrDefault();

            var ticketsDaCesta = cestaVigente!.Itens
                                 .Select(i => i.Ticker)
                                 .ToList();



            var clientes = _context.Clientes.Where(c => c.Ativo).ToList();

            foreach (var cliente in clientes)
            {
                var contaGrafica = _context.ContasGraficas.FirstOrDefault(c => c.ClienteId == cliente.Id);
                var custodiasFilhote = _context.CustodiasFilhotes.Where(c => c.ContaGraficaId == contaGrafica!.Id);

                var ativosAVender = custodiasFilhote
                                    .Where(c => !ticketsDaCesta.Contains(c.Ticker))
                                    .ToList();

                int quantidadeAComprar = 0;
                foreach (var ativo in ativosAVender)
                {
                    quantidadeAComprar = ativo.Quantidade;
                    ativo.Quantidade = 0;
                    ativo.DataUltimaAtualizacao = DateTime.Now;
                }

                _context.SaveChanges();

                _impostoService.CalcularIRsobreVendas(data);

                _motorCompra.ExecutarMotorDeCompra(data);


            }

        }

    }
}
