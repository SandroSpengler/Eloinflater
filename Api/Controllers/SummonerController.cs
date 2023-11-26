using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Namespace;

namespace Api.Controllers
{
    [ApiController]
    [Route("data/[controller]")]
    public class SummonerController : ControllerBase
    {
        private readonly ILogger<SummonerController> _logger;
        private readonly ISummonerService _summonerService;

        public SummonerController(
            ILogger<SummonerController> logger,
            ISummonerService summonerService
        )
        {
            _logger = logger;
            _summonerService = summonerService;
        }

        [HttpGet("{summonerName}")]
        public async Task<ActionResult<SummonerDTO>> GetSummonerByName([FromRoute] string summonerName)
        {
            try
            {
                SummonerDTO? summoner = null;

                summoner = await _summonerService.GetSummonerByName(summonerName);

                if (summoner is not null)
                {
                    return Ok(summoner);
                }

                summoner = await _summonerService.GetSummonerFromRGApi(summonerName);

                if (summoner is not null)
                {
                    return Ok(summoner);
                }

                return NotFound("summoner does not exist");
            }
            catch (System.Exception e)
            {

                _logger.LogError($"SummonerController | GET | / | {e.Message}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("autocomplete/{summonerName}")]
        public async Task<ActionResult<IEnumerable<SummonerDTO>>> GetSummonerAutoComplete([FromRoute] string summonerName)
        {
            try
            {
                IEnumerable<SummonerDTO> summoners = await _summonerService.GetSummonerAutoComplete(summonerName);

                if (summoners.Count() > 0)
                {
                    return Ok(summoners.Take(10));
                }

                return NotFound("summoner does not exist");

            }
            catch (System.Exception e)
            {
                _logger.LogError($"SummonerController | GET | /autocomplete | {e.Message}");

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
