using BlogSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogSystem.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly BlogDbContext _context;

        public AdminController(BlogDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || !user.IsAdmin)
            {
                return Unauthorized();
            }

            ViewBag.IsAdmin = true; // 🔥 Layout içinde admin navbar için kontrol edilen değer

            var allPosts = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .OrderByDescending(p => p.PublishedDate)
                .ToList();

            return View(allPosts);
        }

    }
}
