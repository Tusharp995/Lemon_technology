using E_Commerce_Web_Application.Data;
using E_Commerce_Web_Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_Application.Controllers
{
    [Authorize(Roles = "Seller,Admin,Buyer")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        //public IActionResult Index()
        //{
        //    var products = _context.Products.Include(p => p.Images).ToList();
        //    return View(products);
        //}

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images)
        {
            var user = await _userManager.GetUserAsync(User);
            product.SellerId = user.Id;
            product.Images = new List<ProductImage>();

                foreach (var file in images)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var path = Path.Combine(_env.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.Images.Add(new ProductImage { ImageUrl = "/uploads/" + fileName });
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            

            return View(product);
        }
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.Include(p => p.Images)
                                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Category = updatedProduct.Category;
            product.StockQuantity = updatedProduct.StockQuantity;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Seller,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);

            // Delete images from disk
            foreach (var img in product.Images)
            {
                var filePath = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> Details(int id)
        //{
        //    var product = await _context.Products
        //        .Include(p => p.Images)
        //        .Include(p => p.Reviews) // assuming Review model is related
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    if (product == null) return NotFound();

        //    return View(product);
        //}
        public async Task<IActionResult> Index(string search, string category, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            var pageSize = 6;
            var productsQuery = _context.Products
                .Include(p => p.Images)
                .Include(p => p.Seller)
                .AsQueryable();

            // Only show products created by the logged-in user (if Seller)
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Seller"))
            {
                productsQuery = productsQuery.Where(p => p.SellerId == user.Id);
            }

            // Filters
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            var totalProducts = await productsQuery.CountAsync();
            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.Search = search;
            ViewBag.Categories = await _context.Products.Select(p => p.Category).Distinct().ToListAsync();
            //ViewBag.Category = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Seller)
                .Include(p => p.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int ProductId, int Rating, string Comment)
        {
            var userId = _userManager.GetUserId(User);

            var review = new Review
            {
                ProductId = ProductId,
                Rating = Rating,
                Comment = Comment,
                UserId = userId
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", new { id = ProductId });
        }



        // Add Edit, Delete, Details similarly
    }
}
