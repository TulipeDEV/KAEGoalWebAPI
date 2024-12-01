namespace KAEGoalWebAPI.Services
{
    public interface IAuthService
    {
        Task<string> Register(string username, string password, string role);
        Task<string> Login(string username, string password);
        Task<string> RefreshToken(string refreshtoken);
    }
}
