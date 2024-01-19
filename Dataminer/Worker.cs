using Dataminer.Interfaces;
using System.Diagnostics;

namespace Dataminer
{
    public class Scheduler : BackgroundService
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMiningService _miningService;

        public Scheduler(ILogger<Scheduler> logger, IConfiguration configuration, IMiningService summonerByLeagueService)
        {
            _logger = logger;
            _configuration = configuration;
            _miningService = summonerByLeagueService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string interval = _configuration["Scheduler:DataminerInterval"];

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                Stopwatch watch = Stopwatch.StartNew();

                await _miningService.refreshSummonerByLeague();

                await _miningService.validateAllSummoners();

                watch.Stop();

                _logger.LogInformation($"Finished after {watch.ElapsedMilliseconds} ms");

                await Task.Delay(interval != null ? int.Parse(interval) : 60000, stoppingToken);
            }
        }
    }
}