using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using X.PagedList;
using X.PagedList.Mvc.Core;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace ShopApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;

        public CategoryController(ShopAppAspWebContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 10;
            page = page <= 1 ? 1 : page;
            ViewBag.names = name;
            ViewBag.sorts = sort;

            var categories = await _context.Categories.OrderByDescending(x => x.CategoryId).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine(name);

                categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderByDescending(x => x.CategoryId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        categories = await _context.Categories.OrderBy(x => x.CategoryId).ToListAsync();
                        break;
                    case "Id-DESC":
                        categories = await _context.Categories.OrderByDescending(x => x.CategoryId).ToListAsync();
                        break;

                    case "Name-ASC":
                        categories = await _context.Categories.OrderBy(x => x.CategoryName).ToListAsync();
                        break;
                    case "Name-DESC":
                        categories = await _context.Categories.OrderByDescending(x => x.CategoryName).ToListAsync();
                        break;
                }

            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderBy(x => x.CategoryId).ToListAsync();
                        break;
                    case "Id-DESC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderByDescending(x => x.CategoryId).ToListAsync();
                        break;

                    case "Name-ASC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderBy(x => x.CategoryName).ToListAsync();
                        break;
                    case "Name-DESC":
                        categories = await _context.Categories.Where(x => x.CategoryName.Contains(name)).OrderByDescending(x => x.CategoryName).ToListAsync();
                        break;
                }
            } 

            var pagedCategories = categories.ToPagedList(page, limit);
            return View(pagedCategories);
        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategorySlug,CategoryStatus")] Category category)
        {
            if (_context.Categories.Any(x => x.CategoryName == category.CategoryName))
            {
                _toastNotification.Error("Tên danh mục đã tồn tại trong hệ thống !", 3);
                return RedirectToAction(nameof(Create));
            }
            if (ModelState.IsValid)
            {
                _context.Add(category);
                _toastNotification.Success("Thêm mới danh mục thành công !", 3);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                _toastNotification.Error("Có lỗi xảy ra khi thêm mới !", 3);
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,CategorySlug,CategoryStatus")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    _toastNotification.Success("Cập nhật danh mục thành công !", 3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                if (category?.Products?.Count > 0)
                {
                    _toastNotification.Error("Xóa danh mục thất bại !", 3);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    try
                    {
                        _context.Categories.Remove(category);
                        _toastNotification.Success("Xóa danh mục thành công !", 3);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error deleting file: {e.Message}");
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
