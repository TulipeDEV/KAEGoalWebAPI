namespace KAEGoalWebAPI.Models
{
    public class CointransactionModel
    {
        public string CoinType {  get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
    }
}
