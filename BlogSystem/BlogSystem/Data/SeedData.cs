using BlogSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogSystem.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new BlogDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<BlogDbContext>>());

            // Veriler zaten varsa çık
            if (context.Users.Any() || context.Posts.Any()) return;

            // Kullanıcılar
            var user1 = new User
            {
                Username = "admin",
                Email = "admin@example.com",
                Password = "123456",
                IsAdmin = true
            };

            var user2 = new User { Username = "elifblog", Email = "elif@blog.com", Password = "elif123" };

            context.Users.AddRange(user1, user2);

            // Kategoriler
            var cat1 = new Category { Name = "Yazılım" };
            var cat2 = new Category { Name = "Yapay Zeka" };

            context.Categories.AddRange(cat1, cat2);

            // Etiketler
            var tag1 = new Tag { Name = "C#" };
            var tag2 = new Tag { Name = "Python" };
            var tag3 = new Tag { Name = "Machine Learning" };

            context.Tags.AddRange(tag1, tag2, tag3);

            // Yazılar
            var post1 = new Post
            {
                Title = "C# ile Web Geliştirme",
                Content = "ASP.NET Core kullanarak web uygulamaları geliştirebilirsiniz.",
                User = user1,
                Category = cat1,
                Tags = new List<Tag> { tag1 },
                PublishedDate = DateTime.Now
            };

            var post2 = new Post
            {
                Title = "Yapay Zeka Nedir?",
                Content = "Yapay zeka, makinelerin insan gibi düşünmesini sağlayan bir alandır.",
                User = user2,
                Category = cat2,
                Tags = new List<Tag> { tag2, tag3 },
                PublishedDate = DateTime.Now
            };


            context.Posts.AddRange(post1, post2);

            // Yorumlar
            var comment1 = new Comment { Content = "Harika yazı!", AuthorName = "Ziyaretçi", Post = post1 };
            var comment2 = new Comment { Content = "Teşekkürler, çok faydalı oldu.", AuthorName = "Mehmet", Post = post2 };

            context.Comments.AddRange(comment1, comment2);

            // Kaydet
            context.SaveChanges();
        }
    }
}
