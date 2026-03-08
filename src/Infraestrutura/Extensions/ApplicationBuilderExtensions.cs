using Infraestrutura.Persistencia;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestrutura
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<MySQLDbContext>();

            await DatabaseSeeder.SeedAsync(context);
        }
    }
}