namespace KAEGoalWebAPI.Models
{
    public class Mission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MissionType { get; set; }
        public string PictureUrl { get; set; }
        public decimal CoinReward { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }

        public int? CodeMissionId { get; set; }
        public CodeMission CodeMission {  get; set; } 
    }
}
