namespace KAEGoalWebAPI.Models
{
    public class Workplace
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
