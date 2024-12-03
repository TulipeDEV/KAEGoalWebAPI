using KAEGoalWebAPI.Models;

namespace KAEGoalWebAPI.Services
{
    public interface IAuthService
    {
        Task<string> Register(string username, string password, string role, string firstName, string lastName, string displayName, string profilePictureUrl);
        Task<string> Login(string username, string password);
        Task<string> RefreshToken(string refreshtoken);
        Task<bool> UpdateProfile(int userId, UpdateProfileModel model);
        Task<UserDetailModel> GetUserDetails(int userId);


    }
}
