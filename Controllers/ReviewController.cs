using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Web_Application.Data;
using E_Commerce_Web_Application.Models;

namespace E_Commerce_Web_Application.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: /Review/Create
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(int productId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var review = new Review
            {
                ProductId = productId,
                Rating = rating,
                Comment = comment,
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Product", new { id = productId });

        }

        // GET: /Review/Edit/5
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _context.Reviews.Include(r => r.Product).FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();

            return View(review);
        }

        // POST: /Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rating,Comment")] Review review)
        {
            var existing = await _context.Reviews.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Rating = review.Rating;
            existing.Comment = review.Comment;
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Product", new { id = existing.ProductId });
        }

        // GET: /Review/Delete/5
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.Include(r => r.Product).FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return NotFound();

            return View(review);
        }

        // POST: /Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Seller")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            int productId = review.ProductId;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Product", new { id = productId });
        }
    }
}
