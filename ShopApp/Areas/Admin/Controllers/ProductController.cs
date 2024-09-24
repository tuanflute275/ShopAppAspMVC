using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using X.PagedList;

namespace ShopApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;

        public ProductController(ShopAppAspWebContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }

        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 10;
            page = page <= 1 ? 1 : page;
            ViewBag.names = name;
            ViewBag.sorts = sort;

            var products = await _context.Products.OrderByDescending(x => x.ProductId).Include(p => p.ProductCategory).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                products = await _context.Products
                    .Where(x => x.ProductName.Contains(name))
                    .OrderByDescending(x => x.ProductId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductId).ToListAsync();
                        break;
                    case "Id-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductId).ToListAsync();
                        break;

                    case "Name-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductName).ToListAsync();
                        break;
                    case "Name-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductName).ToListAsync();
                        break;

                    case "Price-ASC":
                        products = await _context.Products.OrderBy(x => x.ProductPrice).ToListAsync();
                        break;
                    case "Price-DESC":
                        products = await _context.Products.OrderByDescending(x => x.ProductPrice).ToListAsync();
                        break;
                }

            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductId).ToListAsync();
                        break;
                    case "Id-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductId).ToListAsync();
                        break;

                    case "Name-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductName).ToListAsync();
                        break;
                    case "Name-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductName).ToListAsync();
                        break;

                    case "Price-ASC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderBy(x => x.ProductPrice).ToListAsync();
                        break;
                    case "Price-DESC":
                        products = await _context.Products.Where(x => x.ProductName.Contains(name)).OrderByDescending(x => x.ProductPrice).ToListAsync();
                        break;
                }
            }

            var pagedData = products.ToPagedList(page, limit);
            return View(pagedData);
        }

        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile fileUpload, [Bind("ProductId,ProductCategoryId,ProductDescription,ProductImage,ProductName,ProductPrice,ProductSalePrice,ProductStatus")] Product product)
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

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "products", fileUpload.FileName);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }
                product.ProductImage = fileUpload.FileName;
            }
            else
            {
                ModelState.AddModelError("ProductImage", "Hình ảnh không được để trống.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(product);
                _toastNotification.Success("Thêm mới sản phẩm thành công !", 3);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            _toastNotification.Error("Có lỗi xảy ra khi thêm mới !", 3);
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.ProductCategoryId);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string? oldImage, IFormFile fileUpload, [Bind("ProductId,ProductCategoryId,ProductDescription,ProductImage,ProductName,ProductPrice,ProductSalePrice,ProductStatus")] Product product)
        {
            if (id != product.ProductId)
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
                    ModelState.AddModelError("ProductImage", "Hình ảnh không hợp lệ. Chỉ chấp nhận các định dạng: jpg, jpeg, png, gif.");
                }

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "products", fileUpload.FileName);

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "products", oldImage);
                    System.IO.File.Delete(pathOldFile);

                }

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                product.ProductImage = fileUpload.FileName;

            }
            else
            {
                product.ProductImage = oldImage;
            }
            if (fileUpload != null || oldImage != null)
            {
                try
                {
                    _context.Update(product);
                    _toastNotification.Success("Cập nhật sản phẩm thành công !", 3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _toastNotification.Success("Xóa sản phẩm thành công !", 3);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
