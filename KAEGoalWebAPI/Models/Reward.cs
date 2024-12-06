namespace KAEGoalWebAPI.Models
{
    public class Reward
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public ICollection<UserReward> UserRewards { get; set; }
    }
}
