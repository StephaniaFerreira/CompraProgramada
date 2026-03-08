using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotorRebalanceamento;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();