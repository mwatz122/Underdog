using Microsoft.AspNetCore.Mvc;
using UnderdogFantasy.Database;
using UnderdogFantasy.Models;
using UnderdogFantasy.Services;

namespace UnderdogFantasy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("import/{sport}")]
        public async Task<IActionResult> Import([FromRoute] Sport sport)
        {
            await _playerService.ImportPlayers(sport);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetPlayersRequestModel getPlayers)
        {
            IEnumerable<Player> players = await _playerService.GetPlayers(getPlayers);

            return Ok(await Task.WhenAll(players.Select(async x => new
            {
                id = x.PlayerId,
                name_brief = x.NameBrief,
                first_name = x.FirstName,
                last_name = x.LastName,
                position = x.Position,
                age = x.Age,
                average_position_age_diff = x.Age - await _playerService.GetPlayerAverageAgeByPosition(x.Position)
            })));
        }
    }
}