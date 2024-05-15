using Microsoft.AspNetCore.Mvc;
using SocialNetworkAnalyzerApi.Services.StatisticService;

namespace SocialNetworkAnalyzerApi.Controllers.Import
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [HttpGet("average-friends/{name}")]
        public async Task<IActionResult> GetAverageFriends(string name, CancellationToken cancellationToken)
        {
            var result = await _statisticService.GetAverageFriends(name, cancellationToken);

            return Ok(result);
        }

        [HttpGet("count/{name}")]
        public async Task<IActionResult> GetCountOfUsers(string name, CancellationToken cancellationToken)
        {
            var result = await _statisticService.GetAllUsersCount(name, cancellationToken);

            return Ok(result);
        }

    }
}