namespace KAEGoalWebAPI.Models
{
    public class AddCoinsRequest
    {
        public int UserId { get; set; }
        public int CoinTypeId { get; set; }
        public decimal Amount { get; set; }
    }

    public class SpendCoinsRequest
    {
        public int UserId { get; set; }
        public int CoinTypeId { get; set; }
        public decimal Amount { get; set; }
    }
}
