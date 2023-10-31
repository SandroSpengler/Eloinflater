using Core.Extensions;
using Core.Interfaces;
using Dataminer;
using Dataminer.Interfaces;
using Dataminer.Services;
using Infrastructure.Extension;
using RestEase.HttpClientFactory;
using System.Net.Http.Headers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;

        services.AddHostedService<Scheduler>();
        services.AddTransient<ISummonerByLeagueService, SummonerByLeagueService>();

        string? protocol = configuration["RiotGames:Protocol"];
        string? region = configuration["RiotGames:Region"];
        string? baseUrl = configuration["RiotGames:BaseUrl"];

        if ((protocol ?? region ?? baseUrl) == null)
        {
            throw new ArgumentException(
                "Missing required environment variable: RiotGames in Dataminer => Program.cs" +
                "\n " +
                "Please check the documentation for a template or take a look at the default appsettings.json"
            );
        }

        string apiUrl = $"{protocol}://{region}.{baseUrl}";

        services.AddHttpClient(apiUrl)
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(apiUrl);
                c.DefaultRequestHeaders
                    .Add("X-Riot-Token", configuration["RiotGames:RGApiKey"]!);
            })
            .UseWithRestEaseClient<IRiotGamesApi>();

        InfrastructureServiceCollection.SetupServiceCollection(services, configuration);
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