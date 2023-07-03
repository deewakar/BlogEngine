using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogEngine.Data;
using BlogEngine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlogEngine.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Post
        [Route("{user}", Name="Post")]
        public async Task<IActionResult> Index()
        {
            // Redirect to Home page if user is not logged in
            string? username = (string) RouteData.Values["user"];
            if(!User.Identity.IsAuthenticated && username == "Post" )
                return RedirectToAction(nameof(Index), "Home");

            /*
            if(String.IsNullOrEmpty(username))
                return RedirectToAction(nameof(Index), "Home");
            */
            ViewData["UserName"] = username;
            var applicationDbContext = _context.Post.Include(p => p.Author);
            return View(await applicationDbContext.Where(p => p.Author.UserName == username)
            .OrderByDescending(p => p.DatePublished).ToListAsync());
        }

        // GET: Post/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize]
        // GET: Post/Create
        public IActionResult Create()
        {
            // getting the UserID from Claims
            ViewData["AuthorId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // current date and time as both publish and modify date when creating a new post
            var dt = new DateTime(DateTime.Now.Ticks);
            ViewData["PublishedDate"] = dt.ToString("yyyy-MM-ddTHH:mm");
            ViewData["ModifiedDate"] = dt.ToString("yyyy-MM-ddTHH:mm");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,DatePublished,DateModified,Tags,AuthorId")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                var currentUser = User.Identity.Name;
                return RedirectToAction(nameof(Index), "Post", new{user = currentUser});
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Post/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "Id", post.AuthorId);
            // current date and time as both publish and modify date when creating a new post
            var dt = new DateTime(DateTime.Now.Ticks);
            ViewData["PublishedDate"] = post.DatePublished.ToString("yyyy-MM-ddTHH:mm");
            ViewData["ModifiedDate"] = dt.ToString("yyyy-MM-ddTHH:mm");

            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,DatePublished,DateModified,Tags,AuthorId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var username = User.Identity.Name;
                return RedirectToAction(nameof(Index), nameof(Post), new {user = username});
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "Id", post.AuthorId);
            return View(post);
        }

        // GET: Post/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Post == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Post'  is null.");
            }
            var post = await _context.Post.FindAsync(id);
            if (post != null)
            {
                _context.Post.Remove(post);
            }

            await _context.SaveChangesAsync();
            var username = User.Identity.Name;
            return RedirectToAction(nameof(Index), "Post", new {user = username});
        }

        private bool PostExists(int id)
        {
            return (_context.Post?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
