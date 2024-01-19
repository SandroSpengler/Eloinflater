using AutoMapper;
using Core;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Namespace;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly ILogger<SummonerController> _logger;
        private readonly IHealthCheckRepository _healthCheckRepository;

        public HealthCheckController(
            ILogger<SummonerController> logger,
            IHealthCheckRepository healthCheckRepository
        )
        {
            _logger = logger;
            _healthCheckRepository = healthCheckRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetSummonerByName()
        {
            try
            {
                bool healthy = await _healthCheckRepository.checkDBConnection();

                if (healthy)
                {
                    return Ok();
                }

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (System.Exception e)
            {

                _logger.LogError($"SummonerController | GET | / | {e.Message}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
