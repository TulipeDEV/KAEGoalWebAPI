namespace KAEGoalWebAPI.Models
{
    public class UserMission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MissionId { get; set; }
        public DateTime CompletedAt { get; set; }

        public User User { get; set; }
        public Mission Mission { get; set; }
    }
}
