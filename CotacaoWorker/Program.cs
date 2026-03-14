using Aplicacao.Interfaces;
using Aplicacao.Services;
using CotacaoWorker;
using CotacaoWorker.Infraestrutura;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Cotacao B3 Worker";
    })
    .ConfigureServices((context, services) =>
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("CotacaoJob");
            //Cria o calendário Brasileiro do ano atual e dos próximos 5 anos sem os feriados.
            q.AddCalendar(
                "br-holidays",
                QuartzHolidayCalendarFactory.CriarCalendarioUtilBrasileiro(),
                replace: true,
                updateTriggers: true);

            q.AddJob<CotacaoJob>(opts => opts.WithIdentity(jobKey));

            //Dispara às 22h de segunda à sexta, considera o calendário criado acima
            q.AddTrigger(t => t
                .ForJob(jobKey)
                .WithIdentity("CotacaoJob-trigger")
                .ModifiedByCalendar("br-holidays")
                .WithCronSchedule("0 0 22 ? * MON-FRI"));
        });

        services.AddQuartzHostedService(q =>
        {
            q.WaitForJobsToComplete = true;
        });

        services.AddTransient<CotacaoJob>();
        services.AddTransient<ICotacaoService, CotacaoService>();
    })
    .Build();

await host.RunAsync();