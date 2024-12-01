namespace KAEGoalWebAPI.Models
{
    public class Coin
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CoinTypeId { get; set; }
        public decimal Balance { get; set; }

        public User User { get; set; }
        public CoinType CoinType { get; set; }
    }
}
