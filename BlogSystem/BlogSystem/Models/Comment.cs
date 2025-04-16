using System.ComponentModel.DataAnnotations;

namespace BlogSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Yorum yazan kullanıcı
        public string AuthorName { get; set; }

        // İlişkiler
        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
