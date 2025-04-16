using BlogSystem.Data;
using BlogSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogSystem.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly BlogDbContext _context;

        public CommentsController(BlogDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Add(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", "Posts", new { id = postId });
            }

            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                CreatedAt = DateTime.Now,
                AuthorName = User.Identity?.Name ?? "Anonim"
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Details", "Posts", new { id = postId });
        }
        [Authorize]
        public IActionResult Index()
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (currentUser == null || !currentUser.IsAdmin)
                return Unauthorized();

            var comments = _context.Comments
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(comments);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (currentUser == null || !currentUser.IsAdmin)
                return Unauthorized();

            var comment = _context.Comments.Find(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
