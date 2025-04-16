using BlogSystem.Data;
using BlogSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogSystem.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly BlogDbContext _context;

        public UsersController(BlogDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (currentUser == null || !currentUser.IsAdmin)
                return Unauthorized();

            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult ToggleAdmin(int id)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (currentUser == null || !currentUser.IsAdmin)
                return Unauthorized();

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.IsAdmin = !user.IsAdmin;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
