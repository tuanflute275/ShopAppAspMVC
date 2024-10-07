using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using System.Security.Claims;
using X.PagedList;

namespace ShopApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;

        public BlogController(ShopAppAspWebContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }
        public async Task<IActionResult> Index(string? name, string? sort, int page =1)
        {
            int limit = 10;
            page = page <= 1 ? 1 : page;
            ViewBag.names = name;
            ViewBag.sorts = sort;

            var blogs = await _context.Blogs.OrderByDescending(x => x.BlogId).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                blogs = await _context.Blogs
                    .Where(x => x.BlogName.Contains(name))
                    .OrderByDescending(x => x.BlogId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        blogs = await _context.Blogs.OrderBy(x => x.BlogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogs = await _context.Blogs.OrderByDescending(x => x.BlogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogs = await _context.Blogs.OrderBy(x => x.BlogName).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogs = await _context.Blogs.OrderByDescending(x => x.BlogName).ToListAsync();
                        break;
                }

            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        blogs = await _context.Blogs.Where(x => x.BlogName.Contains(name)).OrderBy(x => x.BlogId).ToListAsync();
                        break;
                    case "Id-DESC":
                        blogs = await _context.Blogs.Where(x => x.BlogName.Contains(name)).OrderByDescending(x => x.BlogId).ToListAsync();
                        break;

                    case "Name-ASC":
                        blogs = await _context.Blogs.Where(x => x.BlogName.Contains(name)).OrderBy(x => x.BlogName).ToListAsync();
                        break;
                    case "Name-DESC":
                        blogs = await _context.Blogs.Where(x => x.BlogName.Contains(name)).OrderByDescending(x => x.BlogName).ToListAsync();
                        break;
                }
            }

            var pagedData = blogs.ToPagedList(page, limit);
            return View(pagedData);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile fileUpload, [Bind("BlogId,Slug, BlogDescription, BlogImage, BlogName")] Blog blog)
        {
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("BlogImage", "Hình ảnh không hợp lệ. Chỉ chấp nhận các định dạng: jpg, jpeg, png, gif.");
                }

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "blogs", fileUpload.FileName);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }
                blog.BlogImage = fileUpload.FileName;
            }
            else
            {
                ModelState.AddModelError("BlogImage", "Hình ảnh không được để trống.");
            }
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(blog.BlogDescription))
                {
                    blog.CreateBy = HttpContext.User.Identity.Name;
                    blog.CreateDate = DateTime.Now;
                    _context.Add(blog);
                    _toastNotification.Success("Thêm mới bài viết thành công !", 3);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("BlogDescription", "Nội dung bài viết không được để trống.");
                }

            }
            _toastNotification.Error("Có lỗi xảy ra khi thêm mới !", 3);
            return View(blog);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var blog = await _context.Blogs.FindAsync(id);
            if(blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string? oldImage, IFormFile fileUpload, Blog blog)
        {
            if (id != blog.BlogId)
            {
                return NotFound();
            }
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("BlogImage", "Hình ảnh không hợp lệ. Chỉ chấp nhận các định dạng: jpg, jpeg, png, gif.");
                }

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "blogs", fileUpload.FileName);

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "blogs", oldImage);
                    System.IO.File.Delete(pathOldFile);

                }

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                blog.BlogImage = fileUpload.FileName;

            }
            else
            {
                blog.BlogImage = oldImage;
            }
            if (fileUpload != null || oldImage != null)
            {
                try
                {
                    blog.CreateBy = HttpContext.User.Identity.Name;
                    blog.CreateDate = DateTime.Now;
                    _context.Update(blog);
                    _toastNotification.Success("Cập nhật bài viết thành công !", 3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _toastNotification.Error("Có lỗi xảy ra khi cập nhật !", 3);
            return View(blog);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
                _toastNotification.Success("Xóa bài viết thành công !", 3);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}



