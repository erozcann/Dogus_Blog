using BlogSystem.Data;
using BlogSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogSystem.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbContext _context;

        public PostsController(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            ViewBag.IsAdmin = currentUser != null && currentUser.IsAdmin;

            var postsQuery = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId != null)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                postsQuery = postsQuery.Where(p => p.Title.Contains(search));
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Search = search;

            return View(await postsQuery.ToListAsync());
        }



        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, IFormFile imageFile, int[] selectedTags, string newTag, string newCategory)
        {
            if (ModelState.IsValid)
            {
                post.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                post.PublishedDate = DateTime.Now;

                // Görsel işlemi
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    post.ImagePath = "/uploads/" + fileName;
                }

                // Yeni kategori eklenmişse
                if (!string.IsNullOrWhiteSpace(newCategory))
                {
                    var addedCategory = new Category { Name = newCategory };
                    _context.Categories.Add(addedCategory);
                    await _context.SaveChangesAsync();
                    post.CategoryId = addedCategory.Id;
                }

                // Yeni etiket eklenmişse
                post.Tags = new List<Tag>();

                if (!string.IsNullOrWhiteSpace(newTag))
                {
                    var addedTag = new Tag { Name = newTag };
                    _context.Tags.Add(addedTag);
                    await _context.SaveChangesAsync();
                    post.Tags.Add(addedTag);
                }

                foreach (var tagId in selectedTags)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                        post.Tags.Add(tag);
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            ViewBag.Tags = _context.Tags.ToList();
            return View(post);
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Tag(int id)
        {
            var tag = await _context.Tags
                .Include(t => t.Posts)
                    .ThenInclude(p => p.Category)
                .Include(t => t.Posts)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
            {
                return NotFound();
            }

            ViewBag.TagName = tag.Name;
            return View("TagPosts", tag.Posts.ToList());
        }



    }
}
