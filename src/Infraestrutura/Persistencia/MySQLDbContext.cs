using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.Persistencia
{
    public class MySQLDbContext : DbContext, IDbContext
    {
        public MySQLDbContext(DbContextOptions<MySQLDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClienteCadastro> Clientes { get; set; } = null!;
        public DbSet<ContaGrafica> ContasGraficas { get; set; } = null!;

        public DbSet<Cesta> Cestas { get; set; } = null!;
        public DbSet<ItemCesta> ItensCesta { get; set; } = null!;

        public DbSet<ContaMaster> ContaMaster { get; set; } = null!;
        public DbSet<CustodiaMaster> CustodiaMaster { get; set; } = null!;

        public DbSet<Ordem> Ordens { get; set; } = null!;

        public DbSet<CustodiaFilhote> CustodiasFilhotes { get; set; } = null!;
        public DbSet<Cotacao> Cotacoes { get; set; } = null!;
        public DbSet<ArquivoLido> ArquivosLidos { get; set; } = null!;
        public DbSet<DistribuicaoCliente> DistribuicoesCliente { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClienteCadastro>()
                .HasIndex(c => c.Cpf)
                .IsUnique();

            modelBuilder.Entity<ClienteCadastro>()
                .Property(c => c.ValorMensal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Cesta>()
                .HasMany(c => c.Itens)
                .WithOne(i => i.Cesta)
                .HasForeignKey(i => i.CestaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemCesta>()
                .Property(i => i.Percentual)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CustodiaMaster>()
            .HasOne(item => item.ContaMaster)
            .WithMany(conta => conta.ItensCustodia)
            .HasForeignKey(item => item.ContaMasterId);
        }
    }
}
