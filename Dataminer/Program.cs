using Core.Extensions;
using Dataminer;
using Dataminer.Services;
using Infrastructure;
using Infrastructure.Extension;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddHostedService<Scheduler>();
        services.AddTransient<SummonerByLeagueService>();

        ServiceCollectionExtension.SetupServiceCollection(services, configuration);
        CoreServiceCollection.SetupServiceCollection(services);
    })
    .ConfigureLogging(loggerBuilder =>
    {
        loggerBuilder
            .ClearProviders()
            .AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "dd/MM/yy HH:mm:ss:fff ";
            });
    })
    .Build();

host.Run();
