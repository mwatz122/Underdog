namespace UnderdogFantasy.Models
{
    public class GetPlayersRequestModel
    {
        public Sport? Sport { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; }
        public string AgeRange { get; set; }
        public string Position { get; set; }
    }
}