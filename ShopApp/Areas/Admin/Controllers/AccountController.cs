using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AccountController : Controller
    {
        private readonly ShopAppAspWebContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly INotyfService _toastNotification;

        public AccountController(ShopAppAspWebContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, INotyfService notyfService)
        {
            _context = context;
            _environment = environment;
            _toastNotification = notyfService;
        }

        // GET: Admin/Account
        public async Task<IActionResult> Index(string? name, string? sort, int page = 1)
        {
            int limit = 10;
            page = page <= 1 ? 1 : page;
            ViewBag.names = name;
            ViewBag.sorts = sort;

            var accounts = await _context.Accounts.OrderBy(x => x.UserId).ToListAsync();
            if (!string.IsNullOrEmpty(name))
            {
                accounts = await _context.Accounts
                    .Where(x => x.UserName.Contains(name) || x.UserEmail.Contains(name) || x.UserFullName.Contains(name))
                    .OrderByDescending(x => x.UserId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        accounts = await _context.Accounts.OrderBy(x => x.UserId).ToListAsync();
                        break;
                    case "Id-DESC":
                        accounts = await _context.Accounts.OrderByDescending(x => x.UserId).ToListAsync();
                        break;

                    case "UName-ASC":
                        accounts = await _context.Accounts.OrderBy(x => x.UserName).ToListAsync();
                        break;
                    case "UName-DESC":
                        accounts = await _context.Accounts.OrderByDescending(x => x.UserName).ToListAsync();
                        break;
                }

            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(sort))
            {
                ViewBag.sorts = sort;
                switch (sort)
                {
                    case "Id-ASC":
                        accounts = await _context.Accounts
                            .Where(x => x.UserName.Contains(name) || x.UserEmail.Contains(name) || x.UserFullName.Contains(name))
                            .OrderBy(x => x.UserId).ToListAsync();
                        break;
                    case "Id-DESC":
                        accounts = await _context.Accounts.OrderByDescending(x => x.UserId)
                            .Where(x => x.UserName.Contains(name) || x.UserEmail.Contains(name) || x.UserFullName.Contains(name))
                            .ToListAsync();
                        break;

                    case "UName-ASC":
                        accounts = await _context.Accounts.OrderBy(x => x.UserName)
                            .Where(x => x.UserName.Contains(name) || x.UserEmail.Contains(name) || x.UserFullName.Contains(name))
                            .ToListAsync();
                        break;
                    case "UName-DESC":
                        accounts = await _context.Accounts.OrderByDescending(x => x.UserName)
                            .Where(x => x.UserName.Contains(name) || x.UserEmail.Contains(name) || x.UserFullName.Contains(name))
                            .ToListAsync();
                        break;
                }
            }

            var pagedData = accounts.ToPagedList(page, limit);
            return View(pagedData);
        }

        // GET: Admin/Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile fileUpload, [Bind("UserId,UserName,UserPassword,UserFullName,UserPhone,UserAddress,UserAvatar,UserEmail,UserGender,UserActive,UserRole")] Account account)
        {
            // validate form
            if (string.IsNullOrEmpty(account.UserName))
            {
                ModelState.AddModelError("UserName", "Tên tài khoản không được để trống.");
            }
            if (string.IsNullOrEmpty(account.UserName))
            {
                ModelState.AddModelError("UserFullName", "Tên tài khoản không được để trống.");
            }


            if (_context.Accounts.Any(x => x.UserName == account.UserName))
            {
                _toastNotification.Error("Tên tài khoản đã tồn tại trong hệ thống !", 3);
                return RedirectToAction(nameof(Create));
            }
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "accounts", fileUpload.FileName);

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }
                account.UserAvatar = fileUpload.FileName;
            }
            else
            {
                ModelState.AddModelError("UserAvatar", "Hình ảnh không được để trống.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(account);
                _toastNotification.Success("Thêm mới tài khoản thành công !", 3);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            _toastNotification.Error("Có lỗi xảy ra khi thêm mới !", 3);
            return View(account);
        }

        // GET: Admin/Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // POST: Admin/Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string? oldImage, IFormFile fileUpload, [Bind("UserId,UserName,UserPassword,UserFullName,UserPhone,UserAddress,UserAvatar,UserEmail,UserGender,UserActive,UserRole")] Account account)
        {
            if (id != account.UserId)
            {
                return NotFound();
            }
            if (fileUpload != null)
            {
                var rootPath = _environment.ContentRootPath;

                var path = Path.Combine(rootPath, "wwwroot", "Uploads", "accounts", fileUpload.FileName);

                if (!string.IsNullOrEmpty(oldImage))
                {
                    var pathOldFile = Path.Combine(rootPath, "wwwroot", "Uploads", "accounts", oldImage);
                    System.IO.File.Delete(pathOldFile);

                }

                using (var file = System.IO.File.Create(path))
                {
                    fileUpload.CopyTo(file);
                }

                account.UserAvatar = fileUpload.FileName;

            }
            else
            {
                account.UserAvatar = oldImage;
            }
            if (fileUpload != null || oldImage != null)
            {
                try
                {
                    _context.Update(account);
                    _toastNotification.Success("Cập nhật tài khoản thành công !", 3);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.UserId))
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
            return View(account);
        }

        // GET: Admin/Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                _toastNotification.Success("Xóa tài khoản thành công !", 3);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.UserId == id);
        }
    }
}
