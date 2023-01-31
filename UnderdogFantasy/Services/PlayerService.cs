using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnderdogFantasy.Database;
using UnderdogFantasy.Models;

namespace UnderdogFantasy.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly UnderdogFantasyContext _db;
        private static readonly string _playerSourceUrl = "https://api.cbssports.com/fantasy/players/list";
        private static Dictionary<string, double> _playerAverageAgeByPosition = new Dictionary<string, double>();

        public PlayerService(UnderdogFantasyContext db)
        {
            _db = db;
        }

        public async Task ImportPlayers(Sport sport)
        {
            IEnumerable<Player> players = await GetPlayersFromSource(sport);

            await UpdatePlayersInDatabase(players);

            _playerAverageAgeByPosition.Clear();
        }

        public async Task<IEnumerable<Player>> GetPlayers(GetPlayersRequestModel getPlayers)
        {
            IQueryable<Player> players = _db.Players.AsQueryable();

            if (getPlayers.Sport != null)
            {
                players = players.Where(x => x.Sport == getPlayers.Sport.ToString());
            }

            if (!string.IsNullOrWhiteSpace(getPlayers.LastName))
            {
                players = players.Where(x => x.LastName.StartsWith(getPlayers.LastName));
            }

            if (!string.IsNullOrWhiteSpace(getPlayers.Age))
            {
                int age = int.Parse(getPlayers.Age);

                players = players.Where(x => x.Age == age);
            }

            if (!string.IsNullOrWhiteSpace(getPlayers.AgeRange))
            {
                int ageBegin = int.Parse(getPlayers.AgeRange.Split('-')[0]);
                int ageEnd = int.Parse(getPlayers.AgeRange.Split('-')[1]);

                players = players.Where(x => x.Age >= ageBegin && x.Age <= ageEnd);
            }

            if (!string.IsNullOrWhiteSpace(getPlayers.Position))
            {
                players = players.Where(x => x.Position == getPlayers.Position);
            }

            return await players.ToListAsync();
        }

        public async Task<double> GetPlayerAverageAgeByPosition(string position)
        {
            if (_playerAverageAgeByPosition.ContainsKey(position))
            {
                return _playerAverageAgeByPosition[position];
            }
            else
            {
                // Store the key for next time
                IEnumerable<Player> players = await GetPlayers(new GetPlayersRequestModel()
                {
                    Position = position
                });

                double playerAverageAgeByPosition = players.Average(x => x.Age);

                _playerAverageAgeByPosition[position] = playerAverageAgeByPosition;

                return playerAverageAgeByPosition;
            }
        }

        private async Task<IEnumerable<Player>> GetPlayersFromSource(Sport sport)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{_playerSourceUrl}?version=3.0&response_format=JSON&sport={sport.ToString().ToLower()}");

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                string players = JObject.Parse(content)["body"]["players"].ToString();

                switch (sport)
                {
                    case Sport.Baseball:
                        return JsonConvert.DeserializeObject<List<BaseballPlayer>>(players);
                    case Sport.Football:
                        return JsonConvert.DeserializeObject<List<FootballPlayer>>(players);
                    case Sport.Basketball:
                        return JsonConvert.DeserializeObject<List<BasketballPlayer>>(players);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(sport), sport, "This sport is not supported.");
                }
            }
        }

        private async Task UpdatePlayersInDatabase(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                Player existingPlayerRecord = await _db.Players.FindAsync(player.PlayerId);

                if (existingPlayerRecord == null)
                {
                    // Add new player record
                    _db.Players.Add(player);
                }
                else
                {
                    // Update existing player record
                    existingPlayerRecord.FirstName = player.FirstName;
                    existingPlayerRecord.LastName = player.LastName;
                    existingPlayerRecord.Position = player.Position;
                    existingPlayerRecord.Age = player.Age;
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}