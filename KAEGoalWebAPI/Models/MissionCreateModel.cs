namespace KAEGoalWebAPI.Models
{
    public class MissionCreateModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string MissionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string PictureUrl { get; set; }
        public decimal CoinReward { get; set; }

        public string Code { get; set; } //Only for "Code Missions"
    }
}
