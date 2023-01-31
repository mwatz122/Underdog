using UnderdogFantasy.Database;
using UnderdogFantasy.Models;

namespace UnderdogFantasy.Services
{
    public interface IPlayerService
    {
        Task ImportPlayers(Sport sport);
        Task<IEnumerable<Player>> GetPlayers(GetPlayersRequestModel getPlayers);
        Task<double> GetPlayerAverageAgeByPosition(string position);
    }
}