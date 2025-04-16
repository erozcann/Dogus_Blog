using Azure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogSystem.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PublishedDate { get; set; } = DateTime.Now;

        public string? ImagePath { get; set; } // Opsiyonel görsel

        // İlişkiler
        public int UserId { get; set; }
        public User? User { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        public ICollection<Tag>? Tags { get; set; }
    }
}
