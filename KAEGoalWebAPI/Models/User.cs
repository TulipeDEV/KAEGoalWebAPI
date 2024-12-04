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

        public string Displayname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfilePictureUrl { get; set; }
        public ICollection<Coin> Coins { get; set; } = new List<Coin>();

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        public int? WorkplaceId { get; set; }
        public Workplace Workplace { get; set; }

    }
}
