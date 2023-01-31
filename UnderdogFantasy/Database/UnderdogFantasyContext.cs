using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UnderdogFantasy.Extensions;

namespace UnderdogFantasy.Database
{
    public class UnderdogFantasyContext : DbContext
    {
        public DbSet<Player> Players { get; set; }

        public string DbPath { get; }

        public UnderdogFantasyContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "fantasy.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasDiscriminator<string>("Sport")
                .HasValue<BaseballPlayer>("Baseball")
                .HasValue<FootballPlayer>("Football")
                .HasValue<BasketballPlayer>("Basketball");
        }
    }

    [Index(nameof(Sport))]
    [Index(nameof(LastName))]
    [Index(nameof(Age))]
    [Index(nameof(Position))]
    public abstract class Player
    {
        [JsonProperty("id")]
        public long PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sport { get; set; }
        public string Position { get; set; }
        public int Age { get; set; }
        public abstract string NameBrief { get; }
    }

    public class BaseballPlayer : Player
    {
        public override string NameBrief => $"{FirstName.GetFirstCharacter()}. {LastName.GetFirstCharacter()}.";
    }

    public class FootballPlayer : Player
    {
        public override string NameBrief => ($"{FirstName.GetFirstCharacter()}. {LastName}");
    }

    public class BasketballPlayer : Player
    {
        public override string NameBrief => $"{FirstName} {LastName.GetFirstCharacter()}.";
    }
}