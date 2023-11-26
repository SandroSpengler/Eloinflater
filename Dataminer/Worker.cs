using Dataminer.Interfaces;
using Dataminer.Services;
using System.Diagnostics;

namespace Dataminer
{
    public class Scheduler : BackgroundService
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISummonerByLeagueService _summonerByLeagueService;

        public Scheduler(ILogger<Scheduler> logger, IConfiguration configuration, ISummonerByLeagueService summonerByLeagueService)
        {
            _logger = logger;
            _configuration = configuration;
            _summonerByLeagueService = summonerByLeagueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string interval = _configuration["Scheduler:DataminerInterval"];

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                Stopwatch watch = Stopwatch.StartNew();

                await _summonerByLeagueService.validateSummonerByLeague();

                watch.Stop();

                _logger.LogInformation($"Finished after {watch.ElapsedMilliseconds} ms");

                await Task.Delay(interval != null ? int.Parse(interval) : 60000, stoppingToken);
            }
        }
    }
}