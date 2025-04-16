using Microsoft.Extensions.Hosting;

namespace BlogSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;


        public ICollection<Post>? Posts { get; set; }
    }
}
