﻿namespace KAEGoalWebAPI.Models
{
    public class RewardRequestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
