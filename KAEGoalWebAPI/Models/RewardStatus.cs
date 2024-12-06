namespace KAEGoalWebAPI.Models
{
    public enum RewardStatus
    {
        RewardRequested = 1,
        AwaitingApproval = 2,
        PrizeBeingProcured = 3,
        PrizeVerification = 4,
        PrizeReady = 5
    }
}
