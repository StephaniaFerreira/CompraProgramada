using Core.Entities;
using Infraestrutura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(MySQLDbContext context)
        {
            var existeContaMaster = await context.ContaMaster.AnyAsync();

            if (existeContaMaster)
                return;

            var contaMaster = new ContaMaster
            {
                NumeroConta = "MST-000001",
                Tipo = "MASTER"
            };

            context.ContaMaster.Add(contaMaster);

            await context.SaveChangesAsync();
        }
    }
}

