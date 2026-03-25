using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Interfaces
{
    public interface IDbContext
    {
        DbSet<ClienteCadastro> Clientes { get; set; }
        DbSet<ContaGrafica> ContasGraficas { get; set; }
        DbSet<Cesta> Cestas { get; set; }
        DbSet<ItemCesta> ItensCesta { get; set; }
        DbSet<ContaMaster> ContaMaster { get; set; }
        DbSet<CustodiaMaster> CustodiaMaster { get; set; }
        DbSet<Ordem> Ordens { get; set; }
        DbSet<CustodiaFilhote> CustodiasFilhotes { get; set; }
        DbSet<Cotacao> Cotacoes { get; set; }

        int SaveChanges();
    }
}