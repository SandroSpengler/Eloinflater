using AutoMapper;
using Core.Extensions;
using Core.Interfaces;
using Dataminer;
using Dataminer.Interfaces;
using Dataminer.Services;
using Infrastructure.Extension;
using MongoDB.Driver;
using Namespace;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddHostedService<Scheduler>();
        services.AddTransient<IMiningService, MiningService>();
        services.AddAutoMapper(typeof(MappingProfile));

        InfrastructureServiceCollection.SetupServiceCollection(services, configuration);
        CoreServiceCollection.SetupServiceCollection(services, configuration);
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