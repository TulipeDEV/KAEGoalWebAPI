 using System.ComponentModel.DataAnnotations;

namespace KAEGoalWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public ICollection<Coin> Coins { get; set; } = new List<Coin>();
    }
}
