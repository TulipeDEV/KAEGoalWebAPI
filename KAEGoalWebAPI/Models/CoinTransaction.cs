﻿namespace KAEGoalWebAPI.Models
{
    public class CoinTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CoinTypeId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }

        public User User { get; set; }
        public CoinType CoinType { get; set; }
    }
}
