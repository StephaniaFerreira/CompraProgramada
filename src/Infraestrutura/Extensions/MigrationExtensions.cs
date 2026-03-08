using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Infraestrutura.Persistencia;

namespace Infraestrutura
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

           
            using MySQLDbContext context = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            context.Database.Migrate();
        }
    }
}
