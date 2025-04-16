using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}
